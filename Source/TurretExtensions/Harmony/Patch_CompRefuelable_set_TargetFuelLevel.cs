using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.TargetFuelLevel), MethodType.Setter)]
public static class Patch_CompRefuelable_set_TargetFuelLevel
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return ManualPatch_CompRefuelable.FuelCapacityTranspiler(instructions);
    }
}