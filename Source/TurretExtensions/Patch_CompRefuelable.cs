using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions;

public static class Patch_CompRefuelable
{
    public static IEnumerable<CodeInstruction> FuelCapacityTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        var adjustedFuelCapacity = AccessTools.Method(typeof(TurretExtensionsUtility), "AdjustedFuelCapacity");
        foreach (var item in list)
        {
            var codeInstruction = item;
            if (codeInstruction.IsFuelCapacityInstruction())
            {
                yield return codeInstruction;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CompRefuelable), "parent"));
                codeInstruction = new CodeInstruction(OpCodes.Call, adjustedFuelCapacity);
            }

            yield return codeInstruction;
        }
    }

    [HarmonyPatch(typeof(CompRefuelable), "Refuel", typeof(float))]
    public static class Refuel
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return FuelCapacityTranspiler(instructions);
        }
    }

    [HarmonyPatch(typeof(CompRefuelable), "CompInspectStringExtra")]
    public static class CompInspectStringExtra
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return FuelCapacityTranspiler(instructions);
        }
    }

    [HarmonyPatch(typeof(CompRefuelable), "TargetFuelLevel", MethodType.Getter)]
    public static class get_TargetFuelLevel
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return FuelCapacityTranspiler(instructions);
        }
    }

    [HarmonyPatch(typeof(CompRefuelable), "TargetFuelLevel", MethodType.Setter)]
    public static class set_TargetFuelLevel
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return FuelCapacityTranspiler(instructions);
        }
    }

    [HarmonyPatch(typeof(CompRefuelable), "GetFuelCountToFullyRefuel")]
    public static class GetFuelCountToFullyRefuel
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return FuelCapacityTranspiler(instructions);
        }
    }
}