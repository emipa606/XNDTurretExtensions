using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Verb_Shoot), nameof(Verb_Shoot.WarmupComplete))]
public static class Patch_Verb_Shoot_WarmupComplete
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        var getCasterIsPawnInfo = AccessTools.Property(typeof(Verb), nameof(Verb.CasterIsPawn)).GetGetMethod();
        var casterIsActuallyPawn =
            AccessTools.Method(typeof(Patch_Verb_Shoot_WarmupComplete), nameof(CasterIsActuallyPawn));
        var getCasterPawnInfo = AccessTools.Property(typeof(Verb), nameof(Verb.CasterPawn)).GetGetMethod();
        var actualCasterPawnInfo =
            AccessTools.Method(typeof(Patch_Verb_Shoot_WarmupComplete), nameof(ActualCasterPawn));
        foreach (var item in list)
        {
            var codeInstruction = item;
            if (codeInstruction.opcode == OpCodes.Callvirt)
            {
                if (codeInstruction.OperandIs(getCasterIsPawnInfo))
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    codeInstruction = new CodeInstruction(OpCodes.Call, casterIsActuallyPawn);
                }
                else if (codeInstruction.OperandIs(getCasterPawnInfo))
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    codeInstruction = new CodeInstruction(OpCodes.Call, actualCasterPawnInfo);
                }
            }

            yield return codeInstruction;
        }
    }

    private static bool CasterIsActuallyPawn(bool original, Verb instance)
    {
        if (!original)
        {
            return instance.Caster.TryGetComp<CompMannable>()?.MannedNow ?? false;
        }

        return true;
    }

    private static Pawn ActualCasterPawn(Pawn original, Verb instance)
    {
        if (original != null)
        {
            return original;
        }

        var compMannable = instance.Caster.TryGetComp<CompMannable>();
        return compMannable?.ManningPawn;
    }
}