using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_Verb
    {
        [HarmonyPatch(typeof(Verb), nameof(Verb.DrawHighlight))]
        public static class DrawHighlight
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
            {
#if DEBUG
                Log.Message("Transpiler start: Verb.DrawHighlight (no matches)");
#endif

                var instructionList = instructions.ToList();
                var drawRadiusRingInfo = AccessTools.Method(typeof(VerbProperties), nameof(VerbProperties.DrawRadiusRing));
                var tryDrawFiringConeInfo = AccessTools.Method(typeof(DrawHighlight), nameof(TryDrawFiringCone));

                var instructionToBranchTo = instructionList[instructionList.FirstIndexOf(i => i.OperandIs(drawRadiusRingInfo)) + 1];
                var branchLabel = ilGen.DefineLabel();
                instructionToBranchTo.labels.Add(branchLabel);


                yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                yield return new CodeInstruction(OpCodes.Call, tryDrawFiringConeInfo); // ShouldDrawFiringCone(this)
                yield return new CodeInstruction(OpCodes.Brtrue_S, branchLabel);

                /*

                if (!ShouldDrawFiringCone(this))
                    this.verbProps.DrawRadiusRing(this.caster.Position);

                */

                foreach (var ci in instructionList)
                    yield return ci;
            }

            private static bool TryDrawFiringCone(Verb instance)
            {
                if (!(instance.Caster is Building_Turret turret) || !(TurretExtensionsUtility.FiringArcFor(turret) < 360)) return false;

                TurretExtensionsUtility.TryDrawFiringCone(turret, instance.verbProps.range);
                return true;
            }
        }

        [HarmonyPatch(typeof(Verb), nameof(Verb.CanHitTargetFrom))]
        public static class CanHitTargetFrom
        {
            public static void Postfix(Verb __instance, LocalTargetInfo targ, ref bool __result)
            {
                // Also take firing arc into consideration if the caster is a turret
                if (__instance.Caster is Building_Turret && !targ.Cell.WithinFiringArcOf(__instance.caster))
                    __result = false;
            }
        }
    }
}