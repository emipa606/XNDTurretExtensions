using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TurretExtensions;

public static class TurretExtensionsUtility
{
    public static bool IsUpgradable(this ThingDef def, out CompProperties_Upgradable upgradableCompProps)
    {
        upgradableCompProps = def.GetCompProperties<CompProperties_Upgradable>();
        return upgradableCompProps != null;
    }

    public static bool IsUpgradable(this Thing thing, out CompUpgradable upgradableComp)
    {
        upgradableComp = thing.TryGetComp<CompUpgradable>();
        return upgradableComp != null;
    }

    public static bool IsUpgraded(this Thing thing, out CompUpgradable upgradableComp)
    {
        if (thing.IsUpgradable(out upgradableComp))
        {
            return upgradableComp.upgraded;
        }

        return false;
    }

    public static bool AffectedByEMP(this Building_Turret turret)
    {
        if (turret.IsUpgraded(out var upgradableComp) && upgradableComp.Props.affectedByEMP.HasValue)
        {
            return upgradableComp.Props.affectedByEMP.Value;
        }

        return TurretFrameworkExtension.Get(turret.def).affectedByEMP;
    }

    public static float AdjustedFuelCapacity(float baseFuelCapacity, Thing t)
    {
        if (t.IsUpgraded(out var upgradableComp))
        {
            return baseFuelCapacity * upgradableComp.Props.fuelCapacityFactor;
        }

        return baseFuelCapacity;
    }

    public static int AdjustedTurretBurstWarmupTicks(int warmupTicks, Building_Turret turret)
    {
        var turretFrameworkExtension = TurretFrameworkExtension.Get(turret.def);
        float num = warmupTicks;
        if (turretFrameworkExtension.useManningPawnAimingDelayFactor)
        {
            var pawn = turret.TryGetComp<CompMannable>()?.ManningPawn;
            if (pawn != null)
            {
                var statValue = pawn.GetStatValue(StatDefOf.AimingDelayFactor);
                num *= statValue;
            }
        }

        if (turret.IsUpgraded(out var upgradableComp))
        {
            num *= upgradableComp.Props.turretBurstWarmupTimeFactor;
        }

        return Mathf.RoundToInt(num);
    }

    public static string ToStringDegrees(this float degrees)
    {
        return $"{degrees:0.#}°";
    }

    public static bool WithinFiringArcOf(this IntVec3 pos, Thing thing)
    {
        return pos.WithinFiringArcOf(thing.Position, thing.Rotation, FiringArcFor(thing));
    }

    private static bool WithinFiringArcOf(this IntVec3 pos, IntVec3 pos2, Rot4 rot, float firingArc)
    {
        return GenGeo.AngleDifferenceBetween(rot.AsAngle, (pos - pos2).AngleFlat) <= firingArc / 2f;
    }

    public static float FiringArcFor(Thing thing)
    {
        if (!thing.IsUpgraded(out var upgradableComp))
        {
            return TurretFrameworkExtension.Get(thing.def).FiringArc;
        }

        return upgradableComp.Props.FiringArc;
    }

    public static bool TryDrawFiringCone(Building_Turret turret, float distance)
    {
        return TryDrawFiringCone(turret.Position, turret.Rotation, distance, FiringArcFor(turret));
    }

    public static bool TryDrawFiringCone(IntVec3 centre, Rot4 rot, float distance, float arc)
    {
        if (!(arc < 360f))
        {
            return false;
        }

        if (distance > GenRadial.MaxRadialPatternRadius)
        {
            if ((bool)NonPublicFields.GenDraw_maxRadiusMessaged.GetValue(null))
            {
                return false;
            }

            Log.Error($"Cannot draw radius ring of radius {distance}: not enough squares in the precalculated list.");
            NonPublicFields.GenDraw_maxRadiusMessaged.SetValue(null, true);
            return false;
        }

        var list = (List<IntVec3>)NonPublicFields.GenDraw_ringDrawCells.GetValue(null);
        list.Clear();
        var num = GenRadial.NumCellsInRadius(distance);
        for (var i = 0; i < num; i = checked(i + 1))
        {
            var intVec = centre + GenRadial.RadialPattern[i];
            if (intVec.WithinFiringArcOf(centre, rot, arc))
            {
                list.Add(intVec);
            }
        }

        GenDraw.DrawFieldEdges(list);
        return true;
    }

    public static string UpgradeReadoutReportText(StatRequest req)
    {
        var thingDef = (ThingDef)req.Def;
        var compProperties = thingDef.GetCompProperties<CompProperties_Upgradable>();
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("TurretExtensions.TurretUpgradeBenefitsMain".Translate());
        stringBuilder.AppendLine();
        if (compProperties != null)
        {
            var compUpgradable = req.Thing?.TryGetComp<CompUpgradable>();
            var turretFrameworkExtension = TurretFrameworkExtension.Get(thingDef);
            var stuff = GenStuff.DefaultStuffFor(thingDef);
            var hasThing = req.HasThing;
            stringBuilder.AppendLine($"{"Description".Translate()}: {compProperties.description}");
            if (compProperties.costStuffCount > 0 || !compProperties.costList.NullOrEmpty())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"{"TurretExtensions.TurretResourceRequirements".Translate()}:");
                foreach (var item in compUpgradable != null ? compUpgradable.finalCostList : compProperties.costList)
                {
                    stringBuilder.AppendLine($"- {item.count}x {item.thingDef.LabelCap}");
                }

                if (!hasThing && compProperties.costStuffCount > 0)
                {
                    stringBuilder.AppendLine(
                        $"- {compProperties.costStuffCount}x {"StatsReport_Material".Translate()}");
                }
            }

            if (compProperties.constructionSkillPrerequisite > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(
                    $"{"ConstructionNeeded".Translate()}: {compProperties.constructionSkillPrerequisite}");
            }

            if (!compProperties.researchPrerequisites.NullOrEmpty())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"{"ResearchPrerequisites".Translate()}:");
                foreach (var researchPrerequisite in compProperties.researchPrerequisites)
                {
                    stringBuilder.AppendLine($"- {researchPrerequisite.LabelCap}");
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{"TurretExtensions.TurretUpgradeBenefitsUpgradable".Translate()}:");
            if (compProperties.turretGunDef != null)
            {
                stringBuilder.AppendLine(
                    $"- {"Stat_Weapon_Name".Translate()}: {thingDef.building.turretGunDef.LabelCap} => {compProperties.turretGunDef.LabelCap}");
            }

            var list = new List<StatDef>();
            if (!compProperties.statOffsets.NullOrEmpty())
            {
                list.AddRange(compProperties.statOffsets.Select(s => s.stat));
            }

            if (!compProperties.statFactors.NullOrEmpty())
            {
                list.AddRange(compProperties.statFactors.Select(s => s.stat));
            }

            if (list.Any())
            {
                list = list.Distinct().ToList();
                list.SortBy(s => s.LabelCap.RawText);
                foreach (var item2 in list)
                {
                    var toStringStyle = item2.toStringStyle;
                    var toStringNumberSense = item2.toStringNumberSense;
                    var num = hasThing ? req.Thing.GetStatValue(item2) : thingDef.GetStatValueAbstract(item2, stuff);
                    var num2 = num;
                    var statOffsets = compProperties.statOffsets;
                    if (statOffsets != null && statOffsets.StatListContains(item2))
                    {
                        num2 += compProperties.statOffsets.GetStatOffsetFromList(item2);
                    }

                    var statFactors = compProperties.statFactors;
                    if (statFactors != null && statFactors.StatListContains(item2))
                    {
                        num2 *= compProperties.statFactors.GetStatFactorFromList(item2);
                    }

                    stringBuilder.AppendLine(
                        $"- {item2.LabelCap}: {num.ToStringByStyle(toStringStyle, toStringNumberSense)} => {num2.ToStringByStyle(toStringStyle, toStringNumberSense)}");
                }
            }

            if (compProperties.fuelCapacityFactor != 1f)
            {
                var compProperties2 = thingDef.GetCompProperties<CompProperties_Refuelable>();
                if (compProperties2 != null)
                {
                    stringBuilder.AppendLine(
                        $"- {compProperties2.FuelGizmoLabel}: {compProperties2.fuelCapacity} => {Mathf.Round(compProperties2.fuelCapacity * compProperties.fuelCapacityFactor)}");
                }
            }

            if (compProperties.basePowerConsumptionFactor != 1f)
            {
                var compProperties3 = thingDef.GetCompProperties<CompProperties_Power>();
                if (compProperties3 != null)
                {
                    stringBuilder.AppendLine(
                        $"- {"PowerConsumption".Translate()}: {compProperties3.basePowerConsumption:F0} => {Mathf.Round(compProperties3.basePowerConsumption * compProperties.basePowerConsumptionFactor)}");
                }
            }

            if (compProperties.turretBurstWarmupTimeFactor != 1f)
            {
                stringBuilder.AppendLine(
                    $"- {"WarmupTime".Translate()}: {compProperties.turretBurstWarmupTimeFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor)}");
            }

            if (compProperties.turretBurstCooldownTimeFactor != 1f)
            {
                stringBuilder.AppendLine(
                    $"- {"StatsReport_Cooldown".Translate()}: {compProperties.turretBurstCooldownTimeFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor)}");
            }

            if (compProperties.FiringArc != turretFrameworkExtension.FiringArc)
            {
                stringBuilder.AppendLine(
                    $"- {"TurretExtensions.FiringArc".Translate()}: {turretFrameworkExtension.FiringArc.ToStringDegrees()} => {compProperties.FiringArc.ToStringDegrees()}");
            }

            if ((turretFrameworkExtension.manningPawnShootingAccuracyOffset != 0f ||
                 compProperties.manningPawnShootingAccuracyOffset != 0f) && thingDef.HasComp(typeof(CompMannable)))
            {
                stringBuilder.AppendLine(
                    $"- {"TurretExtensions.UserShootingAccuracyModifier".Translate()}: {turretFrameworkExtension.manningPawnShootingAccuracyOffset.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset)} => {compProperties.manningPawnShootingAccuracyOffset.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset)}");
            }

            if (turretFrameworkExtension.canForceAttack != compProperties.canForceAttack &&
                !thingDef.HasComp(typeof(CompMannable)))
            {
                stringBuilder.AppendLine(compProperties.canForceAttack.Value
                    ? $"- {"TurretExtensions.TurretManuallyControllable".Translate()}"
                    : $"- {"TurretExtensions.TurretNotManuallyControllable".Translate()}");
            }
        }
        else
        {
            stringBuilder.AppendLine("TurretExtensions.TurretUpgradeBenefitsNotUpgradable".Translate());
        }

        return stringBuilder.ToString();
    }

    public static bool CanUpgradeTurret(Thing t, Pawn pawn, WorkTypeDef workType, CompUpgradable compUpgradable,
        bool forced)
    {
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (t.Map.designationManager.DesignationOn(t, DesignationDefOf.UpgradeTurret) == null)
        {
            return false;
        }

        if (GenConstruct.FirstBlockingThing(t, pawn) != null)
        {
            return false;
        }

        if (!pawn.CanReserveAndReach(t, PathEndMode.Touch, forced ? Danger.Deadly : pawn.NormalMaxDanger(), 1, -1, null,
                forced))
        {
            return false;
        }

        if (t.IsBurning())
        {
            return false;
        }

        if (workType == WorkTypeDefOf.Construction && pawn.skills.GetSkill(SkillDefOf.Construction).Level <
            compUpgradable.Props.constructionSkillPrerequisite)
        {
            return false;
        }

        return true;
    }
}