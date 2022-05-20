using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_Designator_Cancel
{
    [HarmonyPatch(typeof(Designator_Cancel), "DesignateThing")]
    public static class DesignateThing
    {
        public static void Postfix(Thing t)
        {
            if (t.IsUpgradable(out var upgradableComp) && !upgradableComp.upgraded)
            {
                upgradableComp.Cancel();
            }
        }
    }
}