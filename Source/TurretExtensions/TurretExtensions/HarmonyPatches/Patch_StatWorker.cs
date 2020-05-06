using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_StatWorker
    {
        [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
        public static class GetValueUnfinalized
        {
            public static void Postfix(StatWorker __instance, StatRequest req, ref float __result, StatDef ___stat)
            {
                // Update stats if the turret has been upgraded
                if (!req.Thing.IsUpgraded(out var uC)) return;
                
                var props = uC.Props;
                if (props.statOffsets != null)
                    __result += props.statOffsets.GetStatOffsetFromList(___stat);
                if (props.statFactors != null)
                    __result *= props.statFactors.GetStatFactorFromList(___stat);
            }
        }

        [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
        public static class GetExplanationUnfinalized
        {
            // Update the explanation string if the turret has been upgraded
            public static void Postfix(StatWorker __instance, StatRequest req, ref string __result, StatDef ___stat)
            {
                if (!req.Thing.IsUpgraded(out var uC)) return;
                
                var props = uC.Props;
                var offset = props.statOffsets?.GetStatOffsetFromList(___stat);
                var factor = props.statFactors?.GetStatFactorFromList(___stat);
                if (props.statOffsets != null && offset != 0)
                    __result += "\n\n" + "TurretExtensions.TurretUpgradedText".Translate().CapitalizeFirst() + ": " +
                                ((float) offset).ToStringByStyle(___stat.toStringStyle, ToStringNumberSense.Offset);
                if (props.statFactors != null && factor != 1)
                    __result += "\n\n" + "TurretExtensions.TurretUpgradedText".Translate().CapitalizeFirst() + ": " +
                                ((float) factor).ToStringByStyle(___stat.toStringStyle, ToStringNumberSense.Factor);
            }
        }
    }
}