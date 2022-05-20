using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace TurretExtensions;

public static class Patch_TurretTop
{
    [HarmonyPatch(typeof(TurretTop), "DrawTurret")]
    public static class DrawTurret
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            var turretTopOffsetToUse = AccessTools.Method(typeof(DrawTurret), "TurretTopOffsetToUse");
            var turretTopDrawSizeToUse = AccessTools.Method(typeof(DrawTurret), "TurretTopDrawSizeToUse");
            var turretTopOffsetInfo = AccessTools.Field(typeof(BuildingProperties), "turretTopOffset");
            var turretTopDrawSizeInfo = AccessTools.Field(typeof(BuildingProperties), "turretTopDrawSize");
            foreach (var item in list)
            {
                var codeInstruction = item;
                if (codeInstruction.opcode == OpCodes.Ldflda && codeInstruction.OperandIs(turretTopOffsetInfo))
                {
                    codeInstruction.opcode = OpCodes.Ldfld;
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld,
                        AccessTools.Field(typeof(TurretTop), "parentTurret"));
                    codeInstruction = new CodeInstruction(OpCodes.Call, turretTopOffsetToUse);
                }

                if (codeInstruction.opcode == OpCodes.Ldfld && codeInstruction.OperandIs(turretTopDrawSizeInfo))
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld,
                        AccessTools.Field(typeof(TurretTop), "parentTurret"));
                    codeInstruction = new CodeInstruction(OpCodes.Call, turretTopDrawSizeToUse);
                }

                yield return codeInstruction;
            }
        }

        private static Vector2 TurretTopOffsetToUse(Vector2 original, Building_Turret turret)
        {
            if (!turret.IsUpgraded(out var upgradableComp))
            {
                return original;
            }

            return upgradableComp.Props.turretTopOffset;
        }

        private static float TurretTopDrawSizeToUse(float original, Building_Turret turret)
        {
            if (turret.IsUpgraded(out var upgradableComp) && upgradableComp.Props.turretTopDrawSize != -1f)
            {
                return upgradableComp.Props.turretTopDrawSize;
            }

            return original;
        }
    }
}