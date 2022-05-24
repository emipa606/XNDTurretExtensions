using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

[HarmonyPatch(typeof(StunHandler), "AffectedByEMP", MethodType.Getter)]
public static class Patch_StunHandler
{
    public static bool Prefix(StunHandler __instance, ref bool __result, Thing ___parent)
    {
        if (___parent is not Building_Turret turret)
        {
            return true;
        }

        __result = turret.AffectedByEMP();
        return false;
    }
}