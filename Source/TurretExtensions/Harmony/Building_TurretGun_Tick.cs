using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), "Tick")]
public static class Building_TurretGun_Tick
{
    public static void Postfix(Building_TurretGun __instance, LocalTargetInfo ___forcedTarget)
    {
        var compSmartForcedTarget = __instance.TryGetComp<CompSmartForcedTarget>();
        if (compSmartForcedTarget == null || ___forcedTarget.Thing is not Pawn pawn)
        {
            return;
        }

        switch (pawn.Downed)
        {
            case false when !compSmartForcedTarget.attackingNonDownedPawn &&
                            (!compSmartForcedTarget.Props.onlyApplyWhenUpgraded || __instance.IsUpgraded(out _)):
                compSmartForcedTarget.attackingNonDownedPawn = true;
                break;
            case true when compSmartForcedTarget.attackingNonDownedPawn:
                compSmartForcedTarget.attackingNonDownedPawn = false;
                NonPublicMethods.Building_TurretGun_ResetForcedTarget(__instance);
                break;
        }
    }
}