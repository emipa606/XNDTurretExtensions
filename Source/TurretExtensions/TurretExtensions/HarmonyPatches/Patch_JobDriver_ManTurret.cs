using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_JobDriver_ManTurret
    {
        [HarmonyPatch(typeof(JobDriver_ManTurret), nameof(JobDriver_ManTurret.FindAmmoForTurret))]
        public static class FindAmmoForTurret
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                    Log.Message("Transpiler start: JobDriver_ManTurret.FindAmmoForTurret (1 match)");
#endif


                var instructionList = instructions.ToList();

                var pawnInfo = typeof(JobDriver_ManTurret).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(t => t.GetFields().Any(f => f.FieldType == typeof(Pawn))).GetField("pawn");

                var originalValidatorStore = instructionList.First(i => i.opcode == OpCodes.Stloc_1);

                var updatedValidatorInfo = AccessTools.Method(typeof(FindAmmoForTurret), nameof(UpdatedValidator));

                foreach (var ci in instructionList)
                {
                    var instruction = ci;

                    // Update the validator
                    if (instruction == originalValidatorStore)
                    {
#if DEBUG
                            Log.Message("JobDriver_ManTurret.FindAmmoForTurret match 1 of 1");
#endif

                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldloc_1); // validator
                        yield return new CodeInstruction(OpCodes.Ldloc_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo); // pawn
                        yield return new CodeInstruction(OpCodes.Ldarg_1); // gun
                        yield return new CodeInstruction(OpCodes.Call, updatedValidatorInfo); // UpdatedValidator(original, pawn, gun)
                        
                        instruction = instruction.Clone(); // validator = UpdatedValidator(original, pawn, gun)
                    }

                    yield return instruction;
                }
            }

            private static Predicate<Thing> UpdatedValidator(Predicate<Thing> original, Pawn pawn, Building_TurretGun gun)
            {
                return t =>
                {
                    if (pawn.IsColonist)
                        return original(t);

                    // AI - any projectile that fits and causes harm
                    if (!(t.def.projectileWhenLoaded is ThingDef loadedProjDef)) return false;
                    
                    var projDamageDef = loadedProjDef.projectile.damageDef;
                    return original(t) && gun.gun.TryGetComp<CompChangeableProjectile>().GetParentStoreSettings().AllowedToAccept(t) &&
                           ((bool) NonPublicFields.DamageDef_externalViolence.GetValue(projDamageDef) ||
                            (bool) NonPublicFields.DamageDef_externalViolenceForMechanoids.GetValue(projDamageDef));
                };
            }
        }
    }
}