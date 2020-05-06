using UnityEngine;
using Verse;

namespace TurretExtensions
{
    public class TurretFrameworkExtension : DefModExtension
    {
        private static readonly TurretFrameworkExtension DefaultValues = new TurretFrameworkExtension();
        public bool affectedByEMP = true;
        public bool canForceAttack;
        private readonly float firingArc = 360;

        public TurretGunFaceDirection gunFaceDirectionOnSpawn;
        public float manningPawnShootingAccuracyOffset;
        public bool useManningPawnAimingDelayFactor = true;
        public bool useManningPawnShootingAccuracy = true;

        public float FiringArc => Mathf.Clamp(firingArc, 0, 360);

        public static TurretFrameworkExtension Get(Def def)
        {
            return def.GetModExtension<TurretFrameworkExtension>() ?? DefaultValues;
        }
    }
}