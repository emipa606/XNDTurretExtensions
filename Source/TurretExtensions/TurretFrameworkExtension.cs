using UnityEngine;
using Verse;

namespace TurretExtensions;

public class TurretFrameworkExtension : DefModExtension
{
    private static readonly TurretFrameworkExtension DefaultValues = new TurretFrameworkExtension();

    private readonly float firingArc = 360f;

    public bool affectedByEMP = true;

    public bool canForceAttack;

    public TurretGunFaceDirection gunFaceDirectionOnSpawn;

    public float manningPawnShootingAccuracyOffset;

    public bool useManningPawnAimingDelayFactor = true;

    public bool useManningPawnShootingAccuracy = true;

    public float FiringArc => Mathf.Clamp(firingArc, 0f, 360f);

    public static TurretFrameworkExtension Get(Def def)
    {
        return def.GetModExtension<TurretFrameworkExtension>() ?? DefaultValues;
    }
}