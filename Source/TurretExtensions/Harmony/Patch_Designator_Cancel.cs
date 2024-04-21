using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Designator_Cancel), nameof(Designator_Cancel.DesignateThing))]
public static class Patch_Designator_Cancel
{
    public static void Postfix(Thing t)
    {
        if (t.IsUpgradable(out var upgradableComp) && !upgradableComp.upgraded)
        {
            upgradableComp.Cancel();
        }
    }
}