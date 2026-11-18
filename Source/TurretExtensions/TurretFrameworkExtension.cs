using UnityEngine;
using Verse;

namespace TurretExtensions;

public class TurretFrameworkExtension : DefModExtension
{
    private static readonly TurretFrameworkExtension DefaultValues = new();

    public readonly bool affectedByEMP = true;

    public readonly bool useManningPawnAimingDelayFactor = true;

    public readonly bool useManningPawnShootingAccuracy = true;

    public bool canForceAttack;

    public TurretGunFaceDirection gunFaceDirectionOnSpawn;

    public float manningPawnShootingAccuracyOffset;

    public float FiringArc
    {
        get => Mathf.Clamp(field, 0f, 360f);
    } = 360f;

    public static TurretFrameworkExtension Get(Def def)
    {
        return def.GetModExtension<TurretFrameworkExtension>() ?? DefaultValues;
    }
}