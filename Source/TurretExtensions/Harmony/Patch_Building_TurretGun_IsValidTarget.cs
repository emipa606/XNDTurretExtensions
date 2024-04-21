using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), "IsValidTarget")]
public static class Patch_Building_TurretGun_IsValidTarget
{
    public static void Postfix(Building_TurretGun __instance, Thing t, ref bool __result)
    {
        if (__result && !t.Position.WithinFiringArcOf(__instance))
        {
            __result = false;
        }
    }
}