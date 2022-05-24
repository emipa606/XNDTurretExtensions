using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_Building_Turret
{
    [HarmonyPatch(typeof(Building_Turret), "TargetPriorityFactor", MethodType.Getter)]
    public static class get_TargetPriorityFactor
    {
        public static void Postfix(Building_Turret __instance, ref float __result)
        {
            var compMannable = __instance.TryGetComp<CompMannable>();
            if (compMannable is { MannedNow: true })
            {
                __result = 0f;
            }
        }
    }

    //[HarmonyPatch(typeof(Building_Turret), "PreApplyDamage")]
    //public static class PreApplyDamage
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var instructionList = instructions.ToList();
    //        checked
    //        {
    //            for (var i = 0; i < instructionList.Count; i++)
    //            {
    //                var instruction = instructionList[i];
    //                var value = AccessTools.Method(typeof(StunHandler), "Notify_DamageApplied");
    //                var affectedByEMPInfo = AccessTools.Method(typeof(PreApplyDamage), "AffectedByEMP");
    //                if (instruction.opcode == OpCodes.Ldc_I4_1)
    //                {
    //                    var codeInstruction = instructionList[i + 1];
    //                    if (codeInstruction.opcode == OpCodes.Callvirt && codeInstruction.OperandIs(value))
    //                    {
    //                        yield return new CodeInstruction(OpCodes.Ldarg_0);
    //                        yield return instruction.Clone();
    //                        instruction = new CodeInstruction(OpCodes.Call, affectedByEMPInfo);
    //                    }
    //                }

    //                yield return instruction;
    //            }
    //        }
    //    }

    //    private static bool AffectedByEMP(Building_Turret instance, bool dummyParam)
    //    {
    //        return instance.AffectedByEMP();
    //    }
    //}
}