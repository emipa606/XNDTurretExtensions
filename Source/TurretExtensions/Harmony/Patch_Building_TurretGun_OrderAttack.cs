using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.OrderAttack))]
public static class Patch_Building_TurretGun_OrderAttack
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