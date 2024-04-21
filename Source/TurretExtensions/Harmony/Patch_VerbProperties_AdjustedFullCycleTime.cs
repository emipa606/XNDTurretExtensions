using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.AdjustedFullCycleTime))]
public static class Patch_VerbProperties_AdjustedFullCycleTime
{
    public static bool Prefix(VerbProperties __instance, Verb ownerVerb, ref float __result)
    {
        if (ownerVerb.Caster is not Building_TurretGun building_TurretGun)
        {
            return true;
        }

        __result =
            TurretExtensionsUtility
                .AdjustedTurretBurstWarmupTicks(
                    building_TurretGun.def.building.turretBurstWarmupTime.RandomInRange.SecondsToTicks(),
                    building_TurretGun)
                .TicksToSeconds() + NonPublicMethods.Building_TurretGun_BurstCooldownTime(building_TurretGun) +
            checked((__instance.burstShotCount - 1) * __instance.ticksBetweenBurstShots).TicksToSeconds();
        return false;
    }
}