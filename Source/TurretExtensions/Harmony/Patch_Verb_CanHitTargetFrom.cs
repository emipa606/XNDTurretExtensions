using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Verb), nameof(Verb.CanHitTargetFrom))]
public static class Patch_Verb_CanHitTargetFrom
{
    public static void Postfix(Verb __instance, LocalTargetInfo targ, ref bool __result)
    {
        if (__instance.Caster is Building_Turret && !targ.Cell.WithinFiringArcOf(__instance.caster))
        {
            __result = false;
        }
    }
}