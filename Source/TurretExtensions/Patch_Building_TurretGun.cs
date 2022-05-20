using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_Building_TurretGun
{
    [HarmonyPatch(typeof(Building_TurretGun), "DrawExtraSelectionOverlays")]
    public static class DrawExtraSelectionOverlays
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGen)
        {
            var instructionList = instructions.ToList();
            var drawRadiusRingInfo = AccessTools.Method(typeof(GenDraw), "DrawRadiusRing", new[]
            {
                typeof(IntVec3),
                typeof(float)
            });
            var tryDrawFiringConeInfo = AccessTools.Method(typeof(TurretExtensionsUtility), "TryDrawFiringCone",
                new[]
                {
                    typeof(Building_Turret),
                    typeof(float)
                });
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

    [HarmonyPatch(typeof(Building_TurretGun), "Tick")]
    public static class Tick
    {
        public static void Postfix(Building_TurretGun __instance, LocalTargetInfo ___forcedTarget)
        {
            var compSmartForcedTarget = __instance.TryGetComp<CompSmartForcedTarget>();
            if (compSmartForcedTarget == null || ___forcedTarget.Thing is not Pawn pawn)
            {
                return;
            }

            if (!pawn.Downed && !compSmartForcedTarget.attackingNonDownedPawn &&
                (!compSmartForcedTarget.Props.onlyApplyWhenUpgraded || __instance.IsUpgraded(out var _)))
            {
                compSmartForcedTarget.attackingNonDownedPawn = true;
            }
            else if (pawn.Downed && compSmartForcedTarget.attackingNonDownedPawn)
            {
                compSmartForcedTarget.attackingNonDownedPawn = false;
                NonPublicMethods.Building_TurretGun_ResetForcedTarget(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "SpawnSetup")]
    public static class SpawnSetup
    {
        public static void Postfix(Building_TurretGun __instance, TurretTop ___top)
        {
            switch (TurretFrameworkExtension.Get(__instance.def).gunFaceDirectionOnSpawn)
            {
                case TurretGunFaceDirection.North:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.North.AsAngle);
                    break;
                case TurretGunFaceDirection.East:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.East.AsAngle);
                    break;
                case TurretGunFaceDirection.South:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.South.AsAngle);
                    break;
                case TurretGunFaceDirection.West:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.West.AsAngle);
                    break;
                case TurretGunFaceDirection.Unspecified:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, __instance.Rotation.AsAngle);
                    break;
                default:
                    NonPublicProperties.TurretTop_set_CurRotation(___top, __instance.Rotation.AsAngle);
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "BurstCooldownTime")]
    public static class BurstCooldownTime
    {
        public static void Postfix(Building_TurretGun __instance, ref float __result)
        {
            if (__instance.IsUpgraded(out var upgradableComp))
            {
                __result *= upgradableComp.Props.turretBurstCooldownTimeFactor;
            }
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "IsValidTarget")]
    public static class IsValidTarget
    {
        public static void Postfix(Building_TurretGun __instance, Thing t, ref bool __result)
        {
            if (__result && !t.Position.WithinFiringArcOf(__instance))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "OrderAttack")]
    public static class OrderAttack
    {
        public static bool Prefix(Building_TurretGun __instance, LocalTargetInfo targ)
        {
            if (!targ.IsValid || targ.Cell.WithinFiringArcOf(__instance))
            {
                return true;
            }

            Messages.Message("TurretExtensions.MessageTargetOutsideFiringArc".Translate(), MessageTypeDefOf.RejectInput,
                false);
            return false;
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "TryStartShootSomething")]
    public static class TryStartShootSomething
    {
        public static void Postfix(Building_TurretGun __instance, ref int ___burstWarmupTicksLeft)
        {
            ___burstWarmupTicksLeft =
                TurretExtensionsUtility.AdjustedTurretBurstWarmupTicks(___burstWarmupTicksLeft, __instance);
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "CanSetForcedTarget", MethodType.Getter)]
    public static class CanSetForcedTarget
    {
        public static void Postfix(Building_TurretGun __instance, ref bool __result)
        {
            if (__instance.Faction != Faction.OfPlayer)
            {
                return;
            }

            var turretFrameworkExtension = TurretFrameworkExtension.Get(__instance.def);
            var compUpgradable = __instance.TryGetComp<CompUpgradable>();
            if (compUpgradable == null || compUpgradable.Props.canForceAttack.HasValue &&
                (compUpgradable.upgraded || !turretFrameworkExtension.canForceAttack) &&
                (!compUpgradable.upgraded || !compUpgradable.Props.canForceAttack.Value))
            {
                return;
            }

            if (!__instance.def.HasComp(typeof(CompMannable)))
            {
                __result = true;
            }
            else
            {
                Log.Warning(
                    $"Turret (defName={__instance.def.defName}) has canForceAttack set to true and CompMannable; canForceAttack is redundant in this case.");
            }
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "GetInspectString")]
    public static class GetInspectString
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
}