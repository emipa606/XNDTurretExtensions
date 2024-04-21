using HarmonyLib;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Thing), nameof(Thing.Graphic), MethodType.Getter)]
public static class Patch_Thing_get_Graphic
{
    public static void Postfix(Thing __instance, ref Graphic __result)
    {
        if (__instance.IsUpgraded(out var upgradableComp) && upgradableComp.UpgradedGraphic != null)
        {
            __result = upgradableComp.UpgradedGraphic;
        }
    }
}