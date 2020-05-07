using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public static class Patch_Designator_Cancel
    {
        [HarmonyPatch(typeof(Designator_Cancel), "DesignateThing")]
        public static class DesignateThing
        {
            public static void Postfix(Thing t)
            {
                // Cancelling a turret upgrade drops materials just like when cancelling a construction project
                if (t.IsUpgradable(out var upgradableComp) && !upgradableComp.upgraded) upgradableComp.Cancel();
            }
        }
    }
}