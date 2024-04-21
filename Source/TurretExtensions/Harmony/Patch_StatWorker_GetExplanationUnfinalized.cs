using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
public static class Patch_StatWorker_GetExplanationUnfinalized
{
    public static void Postfix(StatRequest req, ref string __result, StatDef ___stat)
    {
        if (!req.Thing.IsUpgraded(out var upgradableComp))
        {
            return;
        }

        var props = upgradableComp.Props;
        var num = props.statOffsets?.GetStatOffsetFromList(___stat);
        var num2 = props.statFactors?.GetStatFactorFromList(___stat);
        if (props.statOffsets != null && num != 0f && num.HasValue)
        {
            __result += "\n\n" + "TurretExtensions.TurretUpgradedText".Translate().CapitalizeFirst() + ": " +
                        num.Value.ToStringByStyle(___stat.toStringStyle, ToStringNumberSense.Offset);
        }

        if (props.statFactors != null && num2 != 1f && num2.HasValue)
        {
            __result += "\n\n" + "TurretExtensions.TurretUpgradedText".Translate().CapitalizeFirst() + ": " +
                        num2.Value.ToStringByStyle(___stat.toStringStyle, ToStringNumberSense.Factor);
        }
    }
}