﻿using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;


/*
    public override ThingRequest PotentialWorkThingRequest
    {
      get
      {
        return ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame);
      }
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      if (t.Faction != pawn.Faction)
        return (Job) null;
      if (!(t is Frame frame))
        return (Job) null;
      if (GenConstruct.FirstBlockingThing((Thing) frame, pawn) != null)
        return GenConstruct.HandleBlockingThingJob((Thing) frame, pawn, forced);
      bool checkSkills = this.def.workType == WorkTypeDefOf.Construction;
      return !GenConstruct.CanConstruct((Thing) frame, pawn, checkSkills, forced) ? (Job) null : this.ResourceDeliverJobFor(pawn, (IConstructible) frame, true);
    }
*/

namespace TurretExtensions
{
    [UsedImplicitly]
    public class WorkGiver_ConstructDeliverResourcesToTurrets : WorkGiver_ConstructDeliverResources
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForUndefined();

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_Turret turret))
                return null;
                
            if (t.Faction != pawn.Faction)
                return null;
                
            // Not upgradable
            var upgradableComp = t.TryGetComp<CompUpgradable>();
            if (upgradableComp == null)
                return null;

            // Not designated to be upgraded
            if (t.Map.designationManager.DesignationOn(t, DesignationDefOf.UpgradeTurret) == null)
                return null;
                
            if (GenConstruct.FirstBlockingThing(turret, pawn) != null)
                return GenConstruct.HandleBlockingThingJob(turret, pawn, forced);
                
            // Construction skill
            var checkSkill = def.workType == WorkTypeDefOf.Construction;
            if (checkSkill && pawn.skills.GetSkill(SkillDefOf.Construction).Level < upgradableComp.Props.constructionSkillPrerequisite)
                return null;

            return ResourceDeliverJobFor(pawn, upgradableComp, false);
        }
    }
}