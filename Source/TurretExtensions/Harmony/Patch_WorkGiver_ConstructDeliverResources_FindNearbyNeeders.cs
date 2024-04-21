using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "FindNearbyNeeders")]
public static class Patch_WorkGiver_ConstructDeliverResources_FindNearbyNeeders
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return ManualPatch_WorkGiver_ConstructDeliverResources.IConstructibleCastCorrecterTranspiler(instructions,
            OpCodes.Ldarg_3);
    }
}