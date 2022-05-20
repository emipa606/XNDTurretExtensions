using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_StatWorker
{
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class GetValueUnfinalized
    {
        public static void Postfix(StatWorker __instance, StatRequest req, ref float __result, StatDef ___stat)
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

    [HarmonyPatch(typeof(StatWorker), "GetExplanationUnfinalized")]
    public static class GetExplanationUnfinalized
    {
        public static void Postfix(StatWorker __instance, StatRequest req, ref string __result, StatDef ___stat)
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
}