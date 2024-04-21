using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.GetFuelCountToFullyRefuel))]
public static class Patch_CompRefuelable_GetFuelCountToFullyRefuel
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return ManualPatch_CompRefuelable.FuelCapacityTranspiler(instructions);
    }

    public static void Postfix(ref int __result, CompRefuelable __instance)
    {
        __result = Mathf.RoundToInt(__result / TurretExtensionsUtility.AdjustedFuelMultiplier(__instance.parent));
    }
}