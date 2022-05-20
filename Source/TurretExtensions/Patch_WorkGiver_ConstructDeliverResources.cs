using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_WorkGiver_ConstructDeliverResources
{
    public static IEnumerable<CodeInstruction> IConstructibleCastCorrecterTranspiler(
        IEnumerable<CodeInstruction> instructions, OpCode iConstructibleOpcode)
    {
        var instructionList = instructions.ToList();
        var iConstructibleThingInfo =
            AccessTools.Method(typeof(Patch_WorkGiver_ConstructDeliverResources), "IConstructibleThing");
        checked
        {
            for (var i = 0; i < instructionList.Count; i++)
            {
                var codeInstruction = instructionList[i];
                if (codeInstruction.opcode == iConstructibleOpcode)
                {
                    var codeInstruction2 = instructionList[i + 1];
                    if (codeInstruction2.opcode == OpCodes.Castclass || codeInstruction2.opcode == OpCodes.Isinst)
                    {
                        yield return codeInstruction;
                        codeInstruction = new CodeInstruction(OpCodes.Call, iConstructibleThingInfo);
                    }
                }

                yield return codeInstruction;
            }
        }
    }

    private static Thing IConstructibleThing(IConstructible constructible)
    {
        if (constructible is CompUpgradable compUpgradable)
        {
            return compUpgradable.parent;
        }

        return constructible as Thing;
    }

    [HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "FindNearbyNeeders")]
    public static class FindNearbyNeeders
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return IConstructibleCastCorrecterTranspiler(instructions, OpCodes.Ldarg_3);
        }
    }

    [HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "ResourceDeliverJobFor")]
    public static class ResourceDeliverJobFor
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return IConstructibleCastCorrecterTranspiler(instructions, OpCodes.Ldarg_2);
        }
    }
}