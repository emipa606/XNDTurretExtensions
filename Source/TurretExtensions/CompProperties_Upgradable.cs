using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions;

[UsedImplicitly]
public class CompProperties_Upgradable : CompProperties
{
    public readonly float basePowerConsumptionFactor = 1f;

    public readonly float baseResourceDropPct = 0.75f;

    public readonly float destroyedResourceDropPct = 0.25f;

    public readonly float fuelCapacityFactor = 1f;

    public readonly float fuelMultiplierFactor = 1f;

    public readonly float turretBurstCooldownTimeFactor = 1f;

    public readonly float turretBurstWarmupTimeFactor = 1f;

    public readonly float turretTopDrawSize = -1f;
    private readonly EffecterDef upgradeEffect;

    public readonly bool upgradeFailable = true;

    public readonly float upgradeFailMajorChanceFactor = 2f;

    public readonly float upgradeFailMinorResourcesRecovered = 0.5f;

    public readonly float upgradeSuccessChanceFactor = 1f;

    public readonly bool upgradeWorkFactorStuff = true;

    public readonly int workToUpgrade = 1;

    public bool? affectedByEMP;

    public bool? canForceAttack;

    public int constructionSkillPrerequisite;

    public List<ThingDefCountClass> costList;

    public int costStuffCount;

    public string description;

    private float firingArc = -1f;

    public GraphicData graphicData;

    public float manningPawnShootingAccuracyOffset;

    public List<ResearchProjectDef> researchPrerequisites;

    public List<StatModifier> statFactors;

    public List<StatModifier> statOffsets;

    public ThingDef turretGunDef;

    public Vector2 turretTopOffset;

    public string upgradedTurretDescription;

    public bool upgradeFailAlwaysMajor;

    public FloatRange upgradeFailMajorDmgPctRange = new FloatRange(0.1f, 0.5f);

    public float upgradeFailMajorResourcesRecovered;

    public CompProperties_Upgradable()
    {
        compClass = typeof(CompUpgradable);
    }

    public float FiringArc => Mathf.Clamp(firingArc, 0f, 360f);

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);
        var turretFrameworkExtension = TurretFrameworkExtension.Get(parentDef);
        if (!canForceAttack.HasValue)
        {
            canForceAttack = turretFrameworkExtension.canForceAttack;
        }

        if (firingArc == -1f)
        {
            firingArc = turretFrameworkExtension.FiringArc;
        }
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }

        if (!parentDef.MadeFromStuff && costStuffCount > 0)
        {
            yield return $"costStuffCount is greater than 0 but {parentDef} isn't made from stuff";
            costStuffCount = 0;
        }

        if (constructionSkillPrerequisite is >= 0 and <= 20)
        {
            yield break;
        }

        yield return "constructionSkillPrerequisite must be between 0 and 20. Resetting to 0...";
        constructionSkillPrerequisite = 0;
    }

    public EffecterDef UpgradeEffect(Building building)
    {
        return upgradeEffect ?? building.def.repairEffect;
    }
}