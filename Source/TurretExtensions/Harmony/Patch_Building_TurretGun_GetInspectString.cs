using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.GetInspectString))]
public static class Patch_Building_TurretGun_GetInspectString
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        foreach (var item in list)
        {
            if (item.opcode == OpCodes.Ldc_R4 && (float)item.operand == 5f)
            {
                item.operand = float.Epsilon;
            }

            yield return item;
        }
    }
}