using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
public static class Patch_StatWorker_GetValueUnfinalized
{
    public static void Postfix(StatRequest req, ref float __result, StatDef ___stat)
    {
        if (!req.Thing.IsUpgraded(out var upgradableComp))
        {
            return;
        }

        var props = upgradableComp.Props;
        if (props.statOffsets != null)
        {
            __result += props.statOffsets.GetStatOffsetFromList(___stat);
        }

        if (props.statFactors != null)
        {
            __result *= props.statFactors.GetStatFactorFromList(___stat);
        }
    }
}