using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.CompInspectStringExtra))]
public static class Patch_CompRefuelable_CompInspectStringExtra
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return ManualPatch_CompRefuelable.FuelCapacityTranspiler(instructions);
    }
}