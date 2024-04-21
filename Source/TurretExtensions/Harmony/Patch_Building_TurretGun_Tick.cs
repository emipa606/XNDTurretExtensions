using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.Tick))]
public static class Patch_Building_TurretGun_Tick
{
    public static void Postfix(Building_TurretGun __instance, LocalTargetInfo ___forcedTarget)
    {
        var compSmartForcedTarget = __instance.TryGetComp<CompSmartForcedTarget>();
        if (compSmartForcedTarget == null || ___forcedTarget.Thing is not Pawn pawn)
        {
            return;
        }

        if (!pawn.Downed && !compSmartForcedTarget.attackingNonDownedPawn &&
            (!compSmartForcedTarget.Props.onlyApplyWhenUpgraded || __instance.IsUpgraded(out var _)))
        {
            compSmartForcedTarget.attackingNonDownedPawn = true;
        }
        else if (pawn.Downed && compSmartForcedTarget.attackingNonDownedPawn)
        {
            compSmartForcedTarget.attackingNonDownedPawn = false;
            NonPublicMethods.Building_TurretGun_ResetForcedTarget(__instance);
        }
    }
}