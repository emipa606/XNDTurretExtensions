using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.SpawnSetup))]
public static class Patch_Building_TurretGun_SpawnSetup
{
    public static void Postfix(Building_TurretGun __instance, TurretTop ___top)
    {
        switch (TurretFrameworkExtension.Get(__instance.def).gunFaceDirectionOnSpawn)
        {
            case TurretGunFaceDirection.North:
                NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.North.AsAngle);
                break;
            case TurretGunFaceDirection.East:
                NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.East.AsAngle);
                break;
            case TurretGunFaceDirection.South:
                NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.South.AsAngle);
                break;
            case TurretGunFaceDirection.West:
                NonPublicProperties.TurretTop_set_CurRotation(___top, Rot4.West.AsAngle);
                break;
            case TurretGunFaceDirection.Unspecified:
                NonPublicProperties.TurretTop_set_CurRotation(___top, __instance.Rotation.AsAngle);
                break;
            default:
                NonPublicProperties.TurretTop_set_CurRotation(___top, __instance.Rotation.AsAngle);
                break;
        }
    }
}