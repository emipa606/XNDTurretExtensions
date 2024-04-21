using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

public static class ManualPatch_ThingDef
{
    public static class manual_SpecialDisplayStats
    {
        public static Type enumeratorType;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            var turretGunDefInfo = AccessTools.Field(typeof(BuildingProperties), "turretGunDef");
            var reqInfo = AccessTools.Field(enumeratorType, "req");
            var actualTurretGunDefInfo = AccessTools.Method(typeof(manual_SpecialDisplayStats), "ActualTurretGunDef");
            var done = false;
            foreach (var item in list)
            {
                var codeInstruction = item;
                if (!done && codeInstruction.opcode == OpCodes.Ldfld &&
                    (FieldInfo)codeInstruction.operand == turretGunDefInfo)
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, reqInfo);
                    codeInstruction = new CodeInstruction(OpCodes.Call, actualTurretGunDefInfo);
                    done = true;
                }

                yield return codeInstruction;
            }
        }

        private static ThingDef ActualTurretGunDef(ThingDef original, StatRequest req)
        {
            if (!req.HasThing || !req.Thing.IsUpgraded(out var upgradableComp) ||
                upgradableComp.Props.turretGunDef == null)
            {
                return original;
            }

            return upgradableComp.Props.turretGunDef;
        }
    }
}