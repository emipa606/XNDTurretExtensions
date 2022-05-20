using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace TurretExtensions;

public static class HarmonyPatchesUtility
{
    public static bool IsFuelCapacityInstruction(this CodeInstruction instruction)
    {
        if (instruction.opcode == OpCodes.Ldfld)
        {
            return (FieldInfo)instruction.operand ==
                   AccessTools.Field(typeof(CompProperties_Refuelable), "fuelCapacity");
        }

        return false;
    }

    public static bool CallingInstruction(CodeInstruction instruction)
    {
        if (!(instruction.opcode == OpCodes.Call))
        {
            return instruction.opcode == OpCodes.Callvirt;
        }

        return true;
    }

    public static bool LoadFieldInstruction(CodeInstruction instruction)
    {
        if (!(instruction.opcode == OpCodes.Ldfld))
        {
            return instruction.opcode == OpCodes.Ldflda;
        }

        return true;
    }

    public static bool BranchingInstruction(CodeInstruction instruction)
    {
        if (!(instruction.opcode == OpCodes.Bge_Un) && !(instruction.opcode == OpCodes.Bge_Un_S) &&
            !(instruction.opcode == OpCodes.Ble_Un))
        {
            return instruction.opcode == OpCodes.Ble_Un_S;
        }

        return true;
    }
}