using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
public static class Verb_Shoot_TryCastShot
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return Verb_Shoot_WarmupComplete.Transpiler(instructions);
    }
}