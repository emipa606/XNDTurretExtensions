using UnityEngine;
using Verse;

namespace TurretExtensions;

public class TurretFrameworkExtension : DefModExtension
{
    private static readonly TurretFrameworkExtension DefaultValues = new TurretFrameworkExtension();

    public readonly bool affectedByEMP = true;

    private readonly float firingArc = 360f;

    public readonly bool useManningPawnAimingDelayFactor = true;

    public readonly bool useManningPawnShootingAccuracy = true;

    public bool canForceAttack;

    public TurretGunFaceDirection gunFaceDirectionOnSpawn;

    public float manningPawnShootingAccuracyOffset;

    public float FiringArc => Mathf.Clamp(firingArc, 0f, 360f);

    public static TurretFrameworkExtension Get(Def def)
    {
        return def.GetModExtension<TurretFrameworkExtension>() ?? DefaultValues;
    }
}