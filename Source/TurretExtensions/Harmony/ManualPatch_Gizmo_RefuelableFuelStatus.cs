using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

public static class ManualPatch_Gizmo_RefuelableFuelStatus
{
    public static class manual_GizmoOnGUI_Delegate
    {
        public static Type delegateType;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase method, ILGenerator ilGen)
        {
            var list = instructions.ToList();
            var fuelCapacityLocal = ilGen.DeclareLocal(typeof(float));
            var fuelCapacityInfo = AccessTools.Field(typeof(CompProperties_Refuelable), "fuelCapacity");
            var thisInfo = AccessTools.Field(delegateType, "<>4__this");
            var adjustedFuelCapacityInfo =
                AccessTools.Method(typeof(manual_GizmoOnGUI_Delegate), "AdjustedFuelCapacity");
            foreach (var item in list)
            {
                var codeInstruction = item;
                if (codeInstruction.OperandIs(fuelCapacityInfo))
                {
                    var addr = false;
                    if (codeInstruction.opcode == OpCodes.Ldflda)
                    {
                        codeInstruction.opcode = OpCodes.Ldfld;
                        addr = true;
                    }

                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, thisInfo);
                    var codeInstruction2 = new CodeInstruction(OpCodes.Call, adjustedFuelCapacityInfo);
                    if (addr)
                    {
                        yield return codeInstruction2;
                        yield return new CodeInstruction(OpCodes.Stloc_S, fuelCapacityLocal.LocalIndex);
                        codeInstruction = new CodeInstruction(OpCodes.Ldloca_S, fuelCapacityLocal.LocalIndex);
                    }
                    else
                    {
                        codeInstruction = codeInstruction2;
                    }
                }

                yield return codeInstruction;
            }
        }

        private static float AdjustedFuelCapacity(float original, Gizmo_RefuelableFuelStatus instance)
        {
            return TurretExtensionsUtility.AdjustedFuelCapacity(original, instance.refuelable.parent);
        }
    }
}