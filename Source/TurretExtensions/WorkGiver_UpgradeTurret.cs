using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TurretExtensions;

[UsedImplicitly]
public class WorkGiver_UpgradeTurret : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.GetComponent<CompMapTurretExtension>().comps.Select(x => x.parent);
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var compUpgradable = t.TryGetComp<CompUpgradable>();
        if (compUpgradable == null)
        {
            return false;
        }

        if (compUpgradable.upgraded)
        {
            return false;
        }

        if (ModLister.BiotechInstalled && pawn.IsColonyMech && pawn.RaceProps.mechFixedSkillLevel <
            compUpgradable.Props.constructionSkillPrerequisite || pawn.skills?.GetSkill(SkillDefOf.Construction).Level <
            compUpgradable.Props.constructionSkillPrerequisite)
        {
            return false;
        }

        if (!forced &&
            pawn.GetStatValue(StatDefOf.ConstructSuccessChance) * compUpgradable.Props.upgradeSuccessChanceFactor <
            1f && t.HitPoints <=
            Mathf.Floor(t.MaxHitPoints * (1f - compUpgradable.Props.upgradeFailMajorDmgPctRange.TrueMax)))
        {
            return false;
        }

        if (compUpgradable.Props.researchPrerequisites != null &&
            compUpgradable.Props.researchPrerequisites.Any(r => !r.IsFinished))
        {
            return false;
        }

        if (!compUpgradable.SufficientMatsToUpgrade)
        {
            return false;
        }

        if (pawn.CanReserve(t, 1, -1, null, forced))
        {
            return !t.IsBurning();
        }

        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOf.UpgradeTurret, t);
    }
}