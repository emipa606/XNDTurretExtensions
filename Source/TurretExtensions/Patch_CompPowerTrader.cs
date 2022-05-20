using HarmonyLib;
using RimWorld;

namespace TurretExtensions;

public static class Patch_CompPowerTrader
{
    [HarmonyPatch(typeof(CompPowerTrader), "SetUpPowerVars")]
    public static class SetUpPowerVars
    {
        public static void Postfix(CompPowerTrader __instance)
        {
            if (__instance.parent.IsUpgraded(out var upgradableComp))
            {
                __instance.PowerOutput *= upgradableComp.Props.basePowerConsumptionFactor;
            }
        }
    }
}