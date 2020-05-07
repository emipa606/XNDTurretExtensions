using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions
{
    public static class Patch_TurretTop
    {
        [HarmonyPatch(typeof(TurretTop), nameof(TurretTop.DrawTurret))]
        public static class DrawTurret
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                Log.Message("Transpiler start: TurretTop.DrawTurret (2 matches)");
#endif

                var instructionList = instructions.ToList();

                var turretTopOffsetToUse = AccessTools.Method(typeof(DrawTurret), nameof(TurretTopOffsetToUse));
                var turretTopDrawSizeToUse = AccessTools.Method(typeof(DrawTurret), nameof(TurretTopDrawSizeToUse));
                var turretTopOffsetInfo = AccessTools.Field(typeof(BuildingProperties), nameof(BuildingProperties.turretTopOffset));
                var turretTopDrawSizeInfo = AccessTools.Field(typeof(BuildingProperties), nameof(BuildingProperties.turretTopDrawSize));

                foreach (var ci in instructionList)
                {
                    var instruction = ci;

                    if (instruction.opcode == OpCodes.Ldflda && instruction.OperandIs(turretTopOffsetInfo))
                    {
#if DEBUG
                        Log.Message("TurretTop.DrawTurret match 1 of 2");
#endif

                        instruction.opcode = OpCodes.Ldfld;
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TurretTop), "parentTurret"));

                        instruction = new CodeInstruction(OpCodes.Call, turretTopOffsetToUse);
                    }

                    if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(turretTopDrawSizeInfo))
                    {
#if DEBUG
                        Log.Message("TurretTop.DrawTurret match 2 of 2");
#endif

                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TurretTop), "parentTurret"));
                        instruction = new CodeInstruction(OpCodes.Call, turretTopDrawSizeToUse);
                    }

                    yield return instruction;
                }
            }

            private static Vector2 TurretTopOffsetToUse(Vector2 original, Building_Turret turret)
            {
                return turret.IsUpgraded(out var uC) ? uC.Props.turretTopOffset : original;
            }

            private static float TurretTopDrawSizeToUse(float original, Building_Turret turret)
            {
                if (turret.IsUpgraded(out var upgradableComp) && upgradableComp.Props.turretTopDrawSize != -1)
                    return upgradableComp.Props.turretTopDrawSize;

                return original;
            }
        }
    }
}