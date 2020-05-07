using HarmonyLib;
using RimWorld;

namespace TurretExtensions
{
    public static class Patch_CompPowerTrader
    {
        [HarmonyPatch(typeof(CompPowerTrader), nameof(CompPowerTrader.SetUpPowerVars))]
        public static class SetUpPowerVars
        {
            public static void Postfix(CompPowerTrader __instance)
            {
                // If the turret has been upgraded, multiply its power consumption by the upgrade props' power consumption factor
                if (__instance.parent.IsUpgraded(out var upgradableComp))
                    __instance.PowerOutput *= upgradableComp.Props.basePowerConsumptionFactor;
            }
        }
    }
}