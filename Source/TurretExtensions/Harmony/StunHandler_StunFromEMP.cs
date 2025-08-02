using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.StunFromEMP), MethodType.Getter)]
public static class StunHandler_StunFromEMP
{
    public static bool Prefix(ref bool __result, Thing ___parent)
    {
        if (___parent is not Building_Turret turret)
        {
            return true;
        }

        __result = turret.AffectedByEMP();
        return false;
    }
}