using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.DrawExtraSelectionOverlays))]
public static class Patch_Building_TurretGun_DrawExtraSelectionOverlays
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator ilGen)
    {
        var instructionList = instructions.ToList();
        var drawRadiusRingInfo = AccessTools.Method(typeof(GenDraw), nameof(GenDraw.DrawRadiusRing), [
            typeof(IntVec3),
            typeof(float)
        ]);
        var tryDrawFiringConeInfo = AccessTools.Method(typeof(TurretExtensionsUtility),
            nameof(TurretExtensionsUtility.TryDrawFiringCone),
            [
                typeof(Building_Turret),
                typeof(float)
            ]);
        var radRingCount = instructionList.Count(i =>
            HarmonyPatchesUtility.CallingInstruction(i) && (MethodInfo)i.operand == drawRadiusRingInfo);
        var radRingsFound = 0;
        checked
        {
            for (var j = 0; j < instructionList.Count; j++)
            {
                var instruction = instructionList[j];
                if (radRingsFound < radRingCount && HarmonyPatchesUtility.BranchingInstruction(instruction))
                {
                    for (var k = 1; j + k < instructionList.Count; k++)
                    {
                        var codeInstruction = instructionList[j + k];
                        if (HarmonyPatchesUtility.BranchingInstruction(codeInstruction) ||
                            codeInstruction.labels.Contains((Label)instruction.operand))
                        {
                            break;
                        }

                        if (!HarmonyPatchesUtility.CallingInstruction(codeInstruction) ||
                            !codeInstruction.OperandIs(drawRadiusRingInfo))
                        {
                            continue;
                        }

                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return instructionList[j - 2].Clone();
                        yield return new CodeInstruction(OpCodes.Call, tryDrawFiringConeInfo);
                        instruction = new CodeInstruction(OpCodes.Brtrue, instruction.operand);
                        radRingsFound++;
                        break;
                    }
                }

                yield return instruction;
            }
        }
    }
}