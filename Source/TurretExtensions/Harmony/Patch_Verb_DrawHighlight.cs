using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Verb), nameof(Verb.DrawHighlight))]
public static class Patch_Verb_DrawHighlight
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator ilGen)
    {
        var instructionList = instructions.ToList();
        var drawRadiusRingInfo =
            AccessTools.Method(typeof(VerbProperties), nameof(VerbProperties.DrawRadiusRing_NewTemp));
        var tryDrawFiringConeInfo = AccessTools.Method(typeof(Patch_Verb_DrawHighlight), nameof(TryDrawFiringCone));
        var codeInstruction =
            instructionList[checked(instructionList.FirstIndexOf(i => i.OperandIs(drawRadiusRingInfo)) + 1)];
        var branchLabel = ilGen.DefineLabel();
        codeInstruction.labels.Add(branchLabel);
        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Call, tryDrawFiringConeInfo);
        yield return new CodeInstruction(OpCodes.Brtrue_S, branchLabel);
        foreach (var item in instructionList)
        {
            yield return item;
        }
    }

    private static bool TryDrawFiringCone(Verb instance)
    {
        if (instance.Caster is not Building_Turret building_Turret ||
            !(TurretExtensionsUtility.FiringArcFor(building_Turret) < 360f))
        {
            return false;
        }

        TurretExtensionsUtility.TryDrawFiringCone(building_Turret, instance.verbProps.range);
        return true;
    }
}