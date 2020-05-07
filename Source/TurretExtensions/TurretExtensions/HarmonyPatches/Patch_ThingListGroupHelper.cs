using HarmonyLib;
using Verse;

namespace TurretExtensions
{
    [HarmonyPatch(typeof(ThingListGroupHelper), nameof(ThingListGroupHelper.Includes))]
    public class Includes
    {
        public static bool Prefix(ref ThingRequestGroup group, ref ThingDef def, ref bool __result)
        {
            if (def == null || def.IsBlueprint || __result) return true;
            if (group != ThingRequestGroup.Blueprint || !def.HasComp(typeof(CompUpgradable))) return true;

            __result = true;
            return false;
        }
    }
}