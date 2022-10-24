using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_ThingDef
{
    public static class manual_SpecialDisplayStats
    {
        public static Type enumeratorType;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            var turretGunDefInfo = AccessTools.Field(typeof(BuildingProperties), "turretGunDef");
            var reqInfo = AccessTools.Field(enumeratorType, "req");
            var actualTurretGunDefInfo = AccessTools.Method(typeof(manual_SpecialDisplayStats), "ActualTurretGunDef");
            var done = false;
            foreach (var item in list)
            {
                var codeInstruction = item;
                if (!done && codeInstruction.opcode == OpCodes.Ldfld &&
                    (FieldInfo)codeInstruction.operand == turretGunDefInfo)
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, reqInfo);
                    codeInstruction = new CodeInstruction(OpCodes.Call, actualTurretGunDefInfo);
                    done = true;
                }

                yield return codeInstruction;
            }
        }

        private static ThingDef ActualTurretGunDef(ThingDef original, StatRequest req)
        {
            if (!req.HasThing || !req.Thing.IsUpgraded(out var upgradableComp) ||
                upgradableComp.Props.turretGunDef == null)
            {
                return original;
            }

            return upgradableComp.Props.turretGunDef;
        }
    }

    [HarmonyPatch(typeof(ThingDef), "SpecialDisplayStats")]
    public static class SpecialDisplayStats
    {
        public static void Postfix(ThingDef __instance, StatRequest req, ref IEnumerable<StatDrawEntry> __result)
        {
            if (__instance.IsShell)
            {
                var weapon = ThingMaker.MakeThing(__instance.projectileWhenLoaded);
                var projectile = __instance.projectileWhenLoaded.projectile;
                var damageAmount = projectile.GetDamageAmount(weapon);
                var armorPenetration = projectile.GetArmorPenetration(weapon);
                var stoppingPower = projectile.StoppingPower;
                var valueString = projectile.damageDef.label.CapitalizeFirst();
                var explosionRadius = projectile.explosionRadius;
                __result = __result.AddItem(new StatDrawEntry(StatCategoryDefOf.TurretAmmo, "Damage".Translate(),
                    damageAmount.ToString(), "Stat_Thing_Damage_Desc".Translate(), 20));
                __result = __result.AddItem(new StatDrawEntry(StatCategoryDefOf.TurretAmmo,
                    "TurretExtensions.ShellDamageType".Translate(), valueString,
                    "TurretExtensions.ShellDamageType_Desc".Translate(), 19));
                __result = __result.AddItem(new StatDrawEntry(StatCategoryDefOf.TurretAmmo,
                    "ArmorPenetration".Translate(), armorPenetration.ToStringPercent(),
                    "ArmorPenetrationExplanation".Translate(), 18));
                __result = __result.AddItem(new StatDrawEntry(StatCategoryDefOf.TurretAmmo, "StoppingPower".Translate(),
                    stoppingPower.ToString(), "StoppingPowerExplanation".Translate(), 17));
                if (explosionRadius > 0f)
                {
                    __result = __result.AddItem(new StatDrawEntry(StatCategoryDefOf.TurretAmmo,
                        "TurretExtensions.ShellExplosionRadius".Translate(), explosionRadius.ToString(),
                        "TurretExtensions.ShellExplosionRadius_Desc".Translate(), 16));
                }
            }

            var verbProperties = __instance.Verbs.FirstOrDefault(v => v.isPrimary);
            if (verbProperties != null)
            {
                var category = __instance.category != ThingCategory.Pawn
                    ? RimWorld.StatCategoryDefOf.Weapon
                    : RimWorld.StatCategoryDefOf.PawnCombat;
                if (verbProperties.LaunchesProjectile && verbProperties.minRange > 0f)
                {
                    __result = __result.AddItem(new StatDrawEntry(category, "MinimumRange".Translate(),
                        verbProperties.minRange.ToString("F0"), "TurretExtensions.MinimumRange_Desc".Translate(),
                        5385));
                }
            }

            var building = __instance.building;
            if (building == null || !building.IsTurret)
            {
                return;
            }

            var list = new List<StatDrawEntry>();
            if (req.Def is ThingDef def)
            {
                string valueString2;
                CompProperties_Upgradable upgradableCompProps;
                if (req.HasThing && req.Thing.IsUpgradable(out var upgradableComp))
                {
                    valueString2 =
                        (upgradableComp.upgraded
                            ? "TurretExtensions.NoAlreadyUpgraded"
                            : "TurretExtensions.YesClickForDetails").Translate();
                    upgradableCompProps = upgradableComp.Props;
                }
                else
                {
                    valueString2 =
                        (def.IsUpgradable(out upgradableCompProps) ? "TurretExtensions.YesClickForDetails" : "No")
                        .Translate();
                }

                var list2 = new List<Dialog_InfoCard.Hyperlink>();
                if (upgradableCompProps?.turretGunDef != null)
                {
                    list2.Add(new Dialog_InfoCard.Hyperlink(upgradableCompProps.turretGunDef));
                }

                list.Add(new StatDrawEntry(RimWorld.StatCategoryDefOf.BasicsNonPawn,
                    "TurretExtensions.Upgradable".Translate(), valueString2,
                    TurretExtensionsUtility.UpgradeReadoutReportText(req), 999, null, list2));
                var degrees = req.HasThing
                    ? TurretExtensionsUtility.FiringArcFor(req.Thing)
                    : TurretFrameworkExtension.Get(def).FiringArc;
                list.Add(new StatDrawEntry(RimWorld.StatCategoryDefOf.Weapon, "TurretExtensions.FiringArc".Translate(),
                    degrees.ToStringDegrees(), "TurretExtensions.FiringArc_Desc".Translate(), 5380));
            }

            __result = __result.Where(s => s.stat != StatDefOf.RangedWeapon_Cooldown);
            var rangedWeapon_Cooldown = StatDefOf.RangedWeapon_Cooldown;
            list.Add(new StatDrawEntry(rangedWeapon_Cooldown.category, rangedWeapon_Cooldown,
                TurretCooldown(req, building), StatRequest.ForEmpty(), rangedWeapon_Cooldown.toStringNumberSense));
            __result = __result.Where(s => s.LabelCap != "WarmupTime".Translate().CapitalizeFirst());
            list.Add(new StatDrawEntry(RimWorld.StatCategoryDefOf.Weapon, "WarmupTime".Translate(),
                $"{TurretWarmup(req, building):0.##} s", "Stat_Thing_Weapon_MeleeWarmupTime_Desc".Translate(), 3555));
            __result = __result.Concat(list);
        }

        private static float TurretCooldown(StatRequest req, BuildingProperties buildingProps)
        {
            if (req.Thing is Building_TurretGun arg)
            {
                return NonPublicMethods.Building_TurretGun_BurstCooldownTime(arg);
            }

            if (!(buildingProps.turretBurstCooldownTime > 0f))
            {
                return buildingProps.turretGunDef.GetStatValueAbstract(StatDefOf.RangedWeapon_Cooldown);
            }

            return buildingProps.turretBurstCooldownTime;
        }

        private static FloatRange TurretWarmup(StatRequest req, BuildingProperties buildingProps)
        {
            if (req.Thing != null && req.Thing.IsUpgraded(out var upgradableComp))
            {
                return new FloatRange(
                    buildingProps.turretBurstWarmupTime.min * upgradableComp.Props.turretBurstWarmupTimeFactor,
                    buildingProps.turretBurstWarmupTime.max * upgradableComp.Props.turretBurstWarmupTimeFactor);
            }

            return buildingProps.turretBurstWarmupTime;
        }
    }
}