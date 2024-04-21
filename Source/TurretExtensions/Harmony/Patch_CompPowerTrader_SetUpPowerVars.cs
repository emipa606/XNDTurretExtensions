using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(CompPowerTrader), nameof(CompPowerTrader.SetUpPowerVars))]
public static class Patch_CompPowerTrader_SetUpPowerVars
{
    public static void Postfix(CompPowerTrader __instance)
    {
        if (__instance.parent.IsUpgraded(out var upgradableComp))
        {
            __instance.PowerOutput *= upgradableComp.Props.basePowerConsumptionFactor;
        }
    }
}