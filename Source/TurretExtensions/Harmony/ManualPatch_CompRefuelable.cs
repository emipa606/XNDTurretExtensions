using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

public static class ManualPatch_CompRefuelable
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
}