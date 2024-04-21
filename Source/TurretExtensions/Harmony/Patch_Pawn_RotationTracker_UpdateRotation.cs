using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Pawn_RotationTracker), nameof(Pawn_RotationTracker.UpdateRotation))]
public static class Patch_Pawn_RotationTracker_UpdateRotation
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var instructionList = instructions.ToList();
        var draftedGetterInfo = AccessTools.Property(typeof(Pawn), nameof(Pawn.Drafted)).GetGetMethod();
        var canRotateDraftedPawnInfo = AccessTools.Method(typeof(Patch_Pawn_RotationTracker_UpdateRotation),
            nameof(CanRotateDraftedPawn));
        checked
        {
            for (var i = 0; i < instructionList.Count; i++)
            {
                var codeInstruction = instructionList[i];
                if (codeInstruction.opcode == OpCodes.Callvirt &&
                    (MethodInfo)codeInstruction.operand == draftedGetterInfo)
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, instructionList[i - 1].operand);
                    codeInstruction = new CodeInstruction(OpCodes.Call, canRotateDraftedPawnInfo);
                }

                yield return codeInstruction;
            }
        }
    }

    public static bool CanRotateDraftedPawn(bool drafted, Pawn pawn)
    {
        return pawn.MannedThing() == null && drafted;
    }
}