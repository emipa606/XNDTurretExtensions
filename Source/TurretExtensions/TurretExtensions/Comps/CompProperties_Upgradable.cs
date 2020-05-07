using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions
{
    [UsedImplicitly]
    public class CompProperties_Upgradable : CompProperties
    {
        private readonly EffecterDef upgradeEffect = null;
        public bool? affectedByEMP;
        public float basePowerConsumptionFactor = 1;

        // Destroyed
        public float baseResourceDropPct = 0.75f;
        public bool? canForceAttack;
        public int constructionSkillPrerequisite;
        public List<ThingDefCountClass> costList;

        // Costs
        public int costStuffCount;

        // Basics
        public string description;
        public float destroyedResourceDropPct = 0.25f;
        private float firingArc = -1;
        public float fuelCapacityFactor = 1;
        public float fuelMultiplierFactor = 1;
        public GraphicData graphicData;
        public float manningPawnShootingAccuracyOffset = 0;
        public List<ResearchProjectDef> researchPrerequisites;
        public List<StatModifier> statFactors;

        // Results
        public List<StatModifier> statOffsets;
        public float turretBurstCooldownTimeFactor = 1;
        public float turretBurstWarmupTimeFactor = 1;
        public ThingDef turretGunDef;
        public float turretTopDrawSize = -1;
        public Vector2 turretTopOffset;
        public string upgradedTurretDescription;
        public bool upgradeFailable = true;
        public bool upgradeFailAlwaysMajor = false;
        public float upgradeFailMajorChanceFactor = 2;
        public FloatRange upgradeFailMajorDmgPctRange = new FloatRange(0.1f, 0.5f);
        public float upgradeFailMajorResourcesRecovered;
        public float upgradeFailMinorResourcesRecovered = 0.5f;
        public float upgradeSuccessChanceFactor = 1;

        // Job Driver
        public bool upgradeWorkFactorStuff = true;
        public int workToUpgrade = 1;

        public CompProperties_Upgradable()
        {
            compClass = typeof(CompUpgradable);
        }

        public float FiringArc => Mathf.Clamp(firingArc, 0, 360);

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            var extensionValues = TurretFrameworkExtension.Get(parentDef);

            // If canForceAttack is unassigned, match it to DefModExtension
            if (!canForceAttack.HasValue)
                canForceAttack = extensionValues.canForceAttack;

            // If firing arc is unchanged, match it to DefModExtension
            if (firingArc == -1)
                firingArc = extensionValues.FiringArc;
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var e in base.ConfigErrors(parentDef))
                yield return e;

            if (!parentDef.MadeFromStuff && costStuffCount > 0)
            {
                yield return $"costStuffCount is greater than 0 but {parentDef} isn't made from stuff";

                costStuffCount = 0;
            }

            if (constructionSkillPrerequisite >= 0 && constructionSkillPrerequisite <= 20) yield break;

            yield return "constructionSkillPrerequisite must be between 0 and 20. Resetting to 0...";

            constructionSkillPrerequisite = 0;
        }

        public EffecterDef UpgradeEffect(Building building)
        {
            return upgradeEffect ?? building.def.repairEffect;
        }
    }
}