using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TurretExtensions;

[UsedImplicitly]
public class JobDriver_UpgradeTurret : JobDriver
{
    private const TargetIndex TurretInd = TargetIndex.A;

    private CompUpgradable UpgradableComp => TargetThingA.TryGetComp<CompUpgradable>();

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Upgrade();
    }

    private Toil Upgrade()
    {
        var upgrade = new Toil();
        upgrade.tickAction = delegate
        {
            var actor = upgrade.actor;
            actor.skills.Learn(SkillDefOf.Construction, 0.25f);
            var num = actor.GetStatValue(StatDefOf.ConstructionSpeed);
            if (TargetThingA.def.MadeFromStuff)
            {
                num *= TargetThingA.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
            }

            var num2 = actor.GetStatValue(StatDefOf.ConstructSuccessChance) *
                       UpgradableComp.Props.upgradeSuccessChanceFactor;
            if (Rand.Value < 1f - Mathf.Pow(num2, num / UpgradableComp.upgradeWorkTotal) &&
                UpgradableComp.Props.upgradeFailable)
            {
                UpgradableComp.upgradeWorkDone = 0f;
                FailUpgrade(actor, num2, TargetThingA);
                ReadyForNextToil();
            }

            UpgradableComp.upgradeWorkDone += num;
            if (!(UpgradableComp.upgradeWorkDone >= UpgradableComp.upgradeWorkTotal))
            {
                return;
            }

            UpgradableComp.Upgrade();
            Map.designationManager.TryRemoveDesignationOn(TargetThingA, DesignationDefOf.UpgradeTurret);
            actor.records.Increment(RecordDefOf.TurretsUpgraded);
            actor.jobs.EndCurrentJob(JobCondition.Succeeded);
        };
        upgrade.FailOnThingMissingDesignation(TargetIndex.A, DesignationDefOf.UpgradeTurret);
        upgrade.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        upgrade.WithEffect(UpgradableComp.Props.UpgradeEffect((Building)job.GetTarget(TargetIndex.A).Thing),
            TargetIndex.A);
        upgrade.WithProgressBar(TargetIndex.A, () => UpgradableComp.upgradeWorkDone / UpgradableComp.upgradeWorkTotal);
        upgrade.defaultCompleteMode = ToilCompleteMode.Never;
        upgrade.activeSkill = () => SkillDefOf.Construction;
        return upgrade;
    }

    private void FailUpgrade(IBillGiver worker, float successChance, Thing building)
    {
        MoteMaker.ThrowText(building.DrawPos, building.Map, "TurretExtensions.TextMote_UpgradeFail".Translate(), 6f);
        string text = "TurretExtensions.UpgradeFailMinorMessage".Translate(worker.LabelShort, building.Label);
        var yield = UpgradableComp.Props.upgradeFailMinorResourcesRecovered;
        if (UpgradableComp.Props.upgradeFailAlwaysMajor ||
            Rand.Value < (1f - successChance) * UpgradableComp.Props.upgradeFailMajorChanceFactor)
        {
            text = "TurretExtensions.UpgradeFailMajorMessage".Translate(worker.LabelShort, building.Label);
            yield = UpgradableComp.Props.upgradeFailMajorResourcesRecovered;
            var num = building.MaxHitPoints * UpgradableComp.Props.upgradeFailMajorDmgPctRange.RandomInRange;
            building.TakeDamage(new DamageInfo(DamageDefOf.Blunt, num));
        }

        text += ResolveResourceLossMessage(yield);
        RefundResources(yield);
        UpgradableComp.innerContainer.Clear();
        Messages.Message(text, new TargetInfo(building.Position, building.Map), MessageTypeDefOf.NegativeEvent);
    }

    private static string ResolveResourceLossMessage(float yield)
    {
        var text = "";
        if (!(yield < 1f))
        {
            return text;
        }

        text += " ";
        switch (yield)
        {
            case >= 0.8f:
                return text + "TurretExtensions.UpgradeFailResourceLossSmall".Translate();
            case >= 0.35f:
                return text + "TurretExtensions.UpgradeFailResourceLossMedium".Translate();
            case > 0f:
                return text + "TurretExtensions.UpgradeFailResourceLossHigh".Translate();
            default:
                return text + "TurretExtensions.UpgradeFailResourceLossTotal".Translate();
        }
    }

    private void RefundResources(float yield)
    {
        UpgradableComp.innerContainer.TryDropAll(TargetThingA.Position, TargetThingA.Map, ThingPlaceMode.Near,
            delegate(Thing t, int c)
            {
                c = GenMath.RoundRandom(c * yield);
                if (c > 0)
                {
                    t.stackCount = c;
                }
                else
                {
                    t.Destroy();
                }
            });
    }
}