using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.Refuel), typeof(float))]
public static class Patch_CompRefuelable_Refuel
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return ManualPatch_CompRefuelable.FuelCapacityTranspiler(instructions);
    }

    public static void Prefix(ref float amount, CompRefuelable __instance)
    {
        amount *= TurretExtensionsUtility.AdjustedFuelMultiplier(__instance.parent);
    }
}