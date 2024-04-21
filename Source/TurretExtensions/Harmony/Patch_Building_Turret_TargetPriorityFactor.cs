using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_Turret), nameof(Building_Turret.TargetPriorityFactor), MethodType.Getter)]
public static class Patch_Building_Turret_TargetPriorityFactor
{
    public static void Postfix(Building_Turret __instance, ref float __result)
    {
        var compMannable = __instance.TryGetComp<CompMannable>();
        if (compMannable is { MannedNow: true })
        {
            __result = 0f;
        }
    }
}