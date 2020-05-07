using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions
{
    public class Designator_UpgradeTurret : Designator
    {
        private readonly List<Building_Turret> designatedTurrets = new List<Building_Turret>();

        public Designator_UpgradeTurret()
        {
            defaultLabel = "TurretExtensions.DesignatorUpgradeTurret".Translate();
            defaultDesc = "TurretExtensions.DesignatorUpgradeTurretDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("Designations/UpgradeTurret");
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Designate_Haul;
        }

        protected override DesignationDef Designation => DesignationDefOf.UpgradeTurret;

        public override int DraggableDimensions => 2;

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map))
                return false;
            if (!DebugSettings.godMode && loc.Fogged(Map))
                return false;
            if (!UpgradableTurretsInSelection(loc).Any())
                return "TurretExtensions.MessageMustDesignateUpgradableTurrets".Translate();

            return true;
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            return t is Building_Turret turret && turret.Faction == Faction.OfPlayer && turret.IsUpgradable(out var upgradableComp) &&
                   !upgradableComp.upgraded && Map.designationManager.DesignationOn(t, Designation) == null;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var upgradableTurrets = UpgradableTurretsInSelection(c).ToList();
            foreach (var turret in upgradableTurrets)
                if (DebugSettings.godMode)
                    turret.TryGetComp<CompUpgradable>().upgraded = true;
                else
                    DesignateThing(turret);
        }

        public override void DesignateThing(Thing t)
        {
            // Godmode
            if (DebugSettings.godMode)
            {
                var upgradableComp = t.TryGetComp<CompUpgradable>();
                upgradableComp.Upgrade();
                if (upgradableComp.finalCostList == null) return;

                foreach (var thing in upgradableComp.finalCostList)
                {
                    var initThingCount = upgradableComp.innerContainer.TotalStackCountOfDef(thing.thingDef);
                    for (var j = initThingCount; j < thing.count; j++)
                        upgradableComp.innerContainer.TryAdd(ThingMaker.MakeThing(thing.thingDef));
                }
            }
            else
            {
                Map.designationManager.AddDesignation(new Designation(t, Designation));
                designatedTurrets.Add((Building_Turret) t);
            }
        }

        protected override void FinalizeDesignationSucceeded()
        {
            if (!DebugSettings.godMode)
                foreach (var turret in designatedTurrets)
                {
                    NotifyPlayerOfInsufficientSkill(turret);
                    NotifyPlayerOfInsufficientResearch(turret);
                    // NotifyPlayerOfInsufficientMaterials(turret);
                }

            designatedTurrets.Clear();
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        private static void NotifyPlayerOfInsufficientSkill(Thing t)
        {
            var minimumSkill = t.TryGetComp<CompUpgradable>().Props.constructionSkillPrerequisite;
            var freeColonists = Find.CurrentMap.mapPawns.FreeColonists;
            var meetsMinSkill = Enumerable.Any(freeColonists, pawn => pawn.skills.GetSkill(SkillDefOf.Construction).Level >= minimumSkill);

            if (!meetsMinSkill)
                Messages.Message("TurretExtensions.ConstructionSkillTooLowMessage".Translate(Faction.OfPlayer.def.pawnsPlural, t.def.label), MessageTypeDefOf.CautionInput, false);
        }

        private static void NotifyPlayerOfInsufficientResearch(Thing t)
        {
            var researchRequirementsMet = true;
            var researchRequirements = t.TryGetComp<CompUpgradable>().Props.researchPrerequisites;
            var researchProjectsUnfinished = new List<string>();
            if (researchRequirements != null)
                foreach (var research in researchRequirements.Where(research => !research.IsFinished))
                {
                    researchRequirementsMet = false;
                    researchProjectsUnfinished.Add(research.label);
                }

            if (researchRequirementsMet) return;

            string messageText = "TurretExtensions.UpgradeResearchNotMetMessage".Translate(t.def.label) + ": " +
                                 researchProjectsUnfinished.ToCommaList().CapitalizeFirst();
            Messages.Message(messageText, MessageTypeDefOf.CautionInput, false);
        }

        /*
        // Informs the player of missing materials
        private void NotifyPlayerOfInsufficientMaterials(Thing t)
        {
            var upgradableComp = t.TryGetComp<CompUpgradable>();
            if (upgradableComp.finalCostList == null) return;

            var requiredMaterials = upgradableComp.finalCostList.Select(thing => thing.Label).ToList();
            var messageText = "Missing required materials: " + requiredMaterials.ToCommaList().CapitalizeFirst();

            Messages.Message(messageText, MessageTypeDefOf.CautionInput, false);
        }
        */

        private IEnumerable<Building_Turret> UpgradableTurretsInSelection(IntVec3 c)
        {
            if (c.Fogged(Map))
                yield break;

            var thingList = c.GetThingList(Map);
            foreach (var t in thingList.Where(t => CanDesignateThing(t).Accepted))
                yield return (Building_Turret) t;
        }
    }
}