//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using HarmonyLib;
//using RimWorld;
//using Verse;

//namespace TurretExtensions.Harmony;

////[HarmonyPatch(typeof(JobDriver_ManTurret), nameof(JobDriver_ManTurret.FindAmmoForTurret))]
//public static class Patch_JobDriver_ManTurret_FindAmmoForTurret
//{
//    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//    {
//        var list = instructions.ToList();
//        var pawnInfo = typeof(JobDriver_ManTurret).GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic)
//            .First(t => t.GetFields().Any(f => f.FieldType == typeof(Pawn))).GetField("pawn");
//        var originalValidatorStore = list.First(i => i.opcode == OpCodes.Stloc_1);
//        var updatedValidatorInfo =
//            AccessTools.Method(typeof(Patch_JobDriver_ManTurret_FindAmmoForTurret), nameof(UpdatedValidator));
//        foreach (var item in list)
//        {
//            var instruction = item;
//            if (instruction == originalValidatorStore)
//            {
//                yield return instruction;
//                yield return new CodeInstruction(OpCodes.Ldloc_1);
//                yield return new CodeInstruction(OpCodes.Ldloc_0);
//                yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo);
//                yield return new CodeInstruction(OpCodes.Ldarg_1);
//                yield return new CodeInstruction(OpCodes.Call, updatedValidatorInfo);
//                instruction = instruction.Clone();
//            }

//            yield return instruction;
//        }
//    }

//    private static Predicate<Thing> UpdatedValidator(Predicate<Thing> original, Pawn pawn, Building_TurretGun gun)
//    {
//        return delegate(Thing t)
//        {
//            if (pawn.IsColonist)
//            {
//                return original(t);
//            }

//            var projectileWhenLoaded = t.def.projectileWhenLoaded;
//            if (projectileWhenLoaded == null)
//            {
//                return false;
//            }

//            var damageDef = projectileWhenLoaded.projectile.damageDef;
//            return original(t) &&
//                   gun.gun.TryGetComp<CompChangeableProjectile>().GetParentStoreSettings().AllowedToAccept(t) &&
//                   ((bool)NonPublicFields.DamageDef_externalViolence.GetValue(damageDef) ||
//                    (bool)NonPublicFields.DamageDef_externalViolenceForMechanoids.GetValue(damageDef));
//        };
//    }
//}

