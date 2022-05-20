using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

public static class Patch_PlaceWorker_ShowTurretRadius
{
    [HarmonyPatch(typeof(PlaceWorker_ShowTurretRadius), "AllowsPlacing")]
    public static class get_Graphic
    {
        public static bool Prefix(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, ref AcceptanceReport __result)
        {
            var firingArc = TurretFrameworkExtension.Get(checkingDef).FiringArc;
            if (!(firingArc < 360f))
            {
                return true;
            }

            var verbProperties =
                ((ThingDef)checkingDef).building.turretGunDef.Verbs.Find(v => v.verbClass == typeof(Verb_Shoot));
            if (verbProperties.range > 0f)
            {
                TurretExtensionsUtility.TryDrawFiringCone(loc, rot, verbProperties.range, firingArc);
            }

            if (verbProperties.minRange > 0f)
            {
                TurretExtensionsUtility.TryDrawFiringCone(loc, rot, verbProperties.minRange, firingArc);
            }

            __result = true;
            return false;
        }
    }
}