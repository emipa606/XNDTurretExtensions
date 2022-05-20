using HarmonyLib;
using Verse;

namespace TurretExtensions;

public static class Patch_Thing
{
    [HarmonyPatch(typeof(Thing), "Graphic", MethodType.Getter)]
    public static class get_Graphic
    {
        public static void Postfix(Thing __instance, ref Graphic __result)
        {
            if (__instance.IsUpgraded(out var upgradableComp) && upgradableComp.UpgradedGraphic != null)
            {
                __result = upgradableComp.UpgradedGraphic;
            }
        }
    }
}