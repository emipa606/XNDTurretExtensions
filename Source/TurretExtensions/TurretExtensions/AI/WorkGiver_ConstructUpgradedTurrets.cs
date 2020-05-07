using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace TurretExtensions
{
    [UsedImplicitly]
    public class WorkGiver_ConstructUpgradedTurrets : WorkGiver_ConstructDeliverResources
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return CompUpgradable.comps.Where(x => x.parent.Map == pawn.Map).Select(x => x.parent);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            // Different factions
            if (t.Faction != pawn.Faction)
                return null;

            // Not upgradable
            var upgradableComp = t.TryGetComp<CompUpgradable>();
            if (upgradableComp == null)
                return null;

            // Not designated to be upgraded
            if (t.Map.designationManager.DesignationOn(t, DesignationDefOf.UpgradeTurret) == null)
                return null;

            // Blocked
            if (GenConstruct.FirstBlockingThing(t, pawn) != null)
                return GenConstruct.HandleBlockingThingJob(t, pawn, forced);

            // Construction skill
            var checkSkill = def.workType == WorkTypeDefOf.Construction;
            if (checkSkill && pawn.skills.GetSkill(SkillDefOf.Construction).Level < upgradableComp.Props.constructionSkillPrerequisite)
                return null;

            return ResourceDeliverJobFor(pawn, upgradableComp, false);
        }
    }
}