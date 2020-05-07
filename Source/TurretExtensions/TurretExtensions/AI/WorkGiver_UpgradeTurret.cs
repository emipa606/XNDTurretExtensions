using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TurretExtensions
{
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
            var upgradeDesignations = pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.UpgradeTurret).ToList();
            foreach (var des in upgradeDesignations) yield return des.target.Thing;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
#if DEBUG
            Log.Message($"Received Thing {t.Label}, is Turret: {t is Building_Turret}");
#endif
            // Different factions
            if (t.Faction != pawn.Faction)
                return false;

            // Building isn't a turret
            if (!(t is Building_Turret turret))
                return false;

            // Not upgradable
            var upgradableComp = turret?.TryGetComp<CompUpgradable>();
            if (upgradableComp == null)
                return false;

            // Already upgraded
            if (upgradableComp.upgraded)
                return false;

            // Not sufficiently skilled
            if (pawn.skills.GetSkill(SkillDefOf.Construction).Level < upgradableComp.Props.constructionSkillPrerequisite)
                return false;

            // Not forced and there's a risk of destroying the turret
            if (!forced && pawn.GetStatValue(StatDefOf.ConstructSuccessChance) * upgradableComp.Props.upgradeSuccessChanceFactor < 1 &&
                turret.HitPoints <= Mathf.Floor(turret.MaxHitPoints * (1 - upgradableComp.Props.upgradeFailMajorDmgPctRange.TrueMax)))
                return false;

            // Haven't finished research requirements
            if (upgradableComp.Props.researchPrerequisites != null && upgradableComp.Props.researchPrerequisites.Any(r => !r.IsFinished))
                return false;

            // Not enough materials
            if (!upgradableComp.SufficientMatsToUpgrade)
                return false;

            Log.Message("Do we even reach this?");
            // Final condition set - the only set that can return true
            return pawn.CanReserve(turret, 1, -1, null, forced) && !turret.IsBurning();
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(JobDefOf.UpgradeTurret, t);
        }
    }
}