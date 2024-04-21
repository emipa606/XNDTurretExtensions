using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.TryStartShootSomething))]
public static class Patch_Building_TurretGun_TryStartShootSomething
{
    public static void Postfix(Building_TurretGun __instance, ref int ___burstWarmupTicksLeft)
    {
        ___burstWarmupTicksLeft =
            TurretExtensionsUtility.AdjustedTurretBurstWarmupTicks(___burstWarmupTicksLeft, __instance);
    }
}