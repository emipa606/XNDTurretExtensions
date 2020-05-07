using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_Pawn_RotationTracker
    {
        [HarmonyPatch(typeof(Pawn_RotationTracker), nameof(Pawn_RotationTracker.UpdateRotation))]
        public static class UpdateRotation
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                Log.Message("Transpiler start: Pawn_RotationTracker.UpdateRotation (1 match)");
#endif

                var instructionList = instructions.ToList();
                var draftedGetterInfo = AccessTools.Property(typeof(Pawn), nameof(Pawn.Drafted)).GetGetMethod();
                var canRotateDraftedPawnInfo = AccessTools.Method(typeof(UpdateRotation), nameof(CanRotateDraftedPawn));

                for (var i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Look for all calls for pawn.Drafted
                    if (instruction.opcode == OpCodes.Callvirt && (MethodInfo) instruction.operand == draftedGetterInfo)
                    {
#if DEBUG
                        Log.Message("Pawn_RotationTracker.UpdateRotation match 1 of 1");
#endif

                        yield return instruction; // this.pawn.Drafted;
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                        yield return new CodeInstruction(OpCodes.Ldfld, instructionList[i - 1].operand); // this.pawn

                        instruction = new CodeInstruction(OpCodes.Call, canRotateDraftedPawnInfo); // CanRotateDraftedPawn(this.pawn.Drafted, this.pawn)
                    }

                    yield return instruction;
                }
            }

            public static bool CanRotateDraftedPawn(bool drafted, Pawn pawn)
            {
                return pawn.MannedThing() == null && drafted;
            }
        }
    }
}