using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_Gizmo_RefuelableFuelStatus
    {
        public static class manual_GizmoOnGUI_Delegate
        {
            public static Type delegateType;

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method, ILGenerator ilGen)
            {
#if DEBUG
                Log.Message("Transpiler start: Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate (1 match)");
#endif

                var instructionList = instructions.ToList();

                // Add local
                var fuelCapacityLocal = ilGen.DeclareLocal(typeof(float));

                var fuelCapacityInfo = AccessTools.Field(typeof(CompProperties_Refuelable), nameof(CompProperties_Refuelable.fuelCapacity));

                var thisInfo = AccessTools.Field(delegateType, "<>4__this");

                var adjustedFuelCapacityInfo = AccessTools.Method(typeof(manual_GizmoOnGUI_Delegate), nameof(AdjustedFuelCapacity));

                foreach (var ci in instructionList)
                {
                    var instruction = ci;

                    // Adjust all calls to fuel capacity to factor in upgraded status
                    if (instruction.OperandIs(fuelCapacityInfo))
                    {
#if DEBUG
                        Log.Message("Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate match 1 of 1");
#endif

                        var addr = false;
                        if (instruction.opcode == OpCodes.Ldflda)
                        {
                            instruction.opcode = OpCodes.Ldfld;
                            addr = true;
                        }

                        yield return instruction; // this.$this.refuelable.Props.fuelCapacity
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                        yield return new CodeInstruction(OpCodes.Ldfld, thisInfo); // this.$this

                        var callAdjustedFuelCapacity =
                            new CodeInstruction(OpCodes.Call, adjustedFuelCapacityInfo); // AdjustedFuelCapacity(this.$this.refuelable.Props.fuelCapacity, this.$this)
                        if (addr)
                        {
                            yield return callAdjustedFuelCapacity;
                            yield return new CodeInstruction(OpCodes.Stloc_S, fuelCapacityLocal.LocalIndex);

                            instruction = new CodeInstruction(OpCodes.Ldloca_S, fuelCapacityLocal.LocalIndex);
                        }
                        else
                        {
                            instruction = callAdjustedFuelCapacity;
                        }
                    }

                    yield return instruction;
                }
            }

            private static float AdjustedFuelCapacity(float original, Gizmo_RefuelableFuelStatus instance)
            {
                return TurretExtensionsUtility.AdjustedFuelCapacity(original, instance.refuelable.parent);
            }
        }
    }
}