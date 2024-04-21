using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), "CanSetForcedTarget", MethodType.Getter)]
public static class Patch_Building_TurretGun_CanSetForcedTarget
{
    public static void Postfix(Building_TurretGun __instance, ref bool __result)
    {
        if (__instance.Faction != Faction.OfPlayer)
        {
            return;
        }

        var turretFrameworkExtension = TurretFrameworkExtension.Get(__instance.def);
        var compUpgradable = __instance.TryGetComp<CompUpgradable>();
        if (compUpgradable == null || compUpgradable.Props.canForceAttack.HasValue &&
            (compUpgradable.upgraded || !turretFrameworkExtension.canForceAttack) &&
            (!compUpgradable.upgraded || !compUpgradable.Props.canForceAttack.Value))
        {
            return;
        }

        if (!__instance.def.HasComp(typeof(CompMannable)))
        {
            __result = true;
        }
        else
        {
            Log.Warning(
                $"Turret (defName={__instance.def.defName}) has canForceAttack set to true and CompMannable; canForceAttack is redundant in this case.");
        }
    }
}