using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), "BurstCooldownTime")]
public static class Patch_Building_TurretGun_BurstCooldownTime
{
    public static void Postfix(Building_TurretGun __instance, ref float __result)
    {
        if (__instance.IsUpgraded(out var upgradableComp))
        {
            __result *= upgradableComp.Props.turretBurstCooldownTimeFactor;
        }
    }
}