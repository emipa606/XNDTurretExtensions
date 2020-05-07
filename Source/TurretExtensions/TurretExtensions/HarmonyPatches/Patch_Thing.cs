using HarmonyLib;
using Verse;

namespace TurretExtensions
{
    public static class Patch_Thing
    {
        [HarmonyPatch(typeof(Thing), nameof(Thing.Graphic), MethodType.Getter)]
        public static class get_Graphic
        {
            public static void Postfix(Thing __instance, ref Graphic __result)
            {
                // Replace the graphic with the upgraded graphic if applicable
                if (__instance.IsUpgraded(out var uC) && uC.UpgradedGraphic != null)
                    __result = uC.UpgradedGraphic;
            }
        }
    }
}