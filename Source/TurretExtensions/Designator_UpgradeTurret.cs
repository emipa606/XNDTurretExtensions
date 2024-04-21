using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions;

public class Designator_UpgradeTurret : Designator
{
    private readonly List<Building_Turret> designatedTurrets = [];

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
        {
            return false;
        }

        if (!DebugSettings.godMode && loc.Fogged(Map))
        {
            return false;
        }

        if (!UpgradableTurretsInSelection(loc).Any())
        {
            return "TurretExtensions.MessageMustDesignateUpgradableTurrets".Translate();
        }

        return true;
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        return t is Building_Turret building_Turret && building_Turret.Faction == Faction.OfPlayer &&
               building_Turret.IsUpgradable(out var upgradableComp) && !upgradableComp.upgraded &&
               Map.designationManager.DesignationOn(t, Designation) == null;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        foreach (var item in UpgradableTurretsInSelection(c).ToList())
        {
            if (DebugSettings.godMode)
            {
                item.TryGetComp<CompUpgradable>().upgraded = true;
            }
            else
            {
                DesignateThing(item);
            }
        }
    }

    public override void DesignateThing(Thing t)
    {
        if (DebugSettings.godMode)
        {
            var compUpgradable = t.TryGetComp<CompUpgradable>();
            compUpgradable.Upgrade();
            if (compUpgradable.finalCostList == null)
            {
                return;
            }

            {
                foreach (var finalCost in compUpgradable.finalCostList)
                {
                    for (var i = compUpgradable.innerContainer.TotalStackCountOfDef(finalCost.thingDef);
                         i < finalCost.count;
                         i = checked(i + 1))
                    {
                        compUpgradable.innerContainer.TryAdd(ThingMaker.MakeThing(finalCost.thingDef));
                    }
                }

                return;
            }
        }

        Map.designationManager.AddDesignation(new Designation(t, Designation));
        designatedTurrets.Add((Building_Turret)t);
    }

    protected override void FinalizeDesignationSucceeded()
    {
        if (!DebugSettings.godMode)
        {
            foreach (var designatedTurret in designatedTurrets)
            {
                NotifyPlayerOfInsufficientSkill(designatedTurret);
                NotifyPlayerOfInsufficientResearch(designatedTurret);
            }
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

        if (ModLister.BiotechInstalled && Find.CurrentMap.mapPawns.AllPawns.Any(pawn =>
                pawn.IsColonyMech && pawn.RaceProps.mechFixedSkillLevel >= minimumSkill))
        {
            return;
        }

        if (!Enumerable.Any(Find.CurrentMap.mapPawns.FreeColonists,
                pawn => pawn.skills.GetSkill(SkillDefOf.Construction).Level >= minimumSkill))
        {
            Messages.Message(
                "TurretExtensions.ConstructionSkillTooLowMessage".Translate(Faction.OfPlayer.def.pawnsPlural,
                    t.def.label), MessageTypeDefOf.CautionInput, false);
        }
    }

    private static void NotifyPlayerOfInsufficientResearch(Thing t)
    {
        var missingResearch = true;
        var researchPrerequisites = t.TryGetComp<CompUpgradable>().Props.researchPrerequisites;
        var list = new List<string>();
        if (researchPrerequisites != null)
        {
            foreach (var item in researchPrerequisites.Where(research => !research.IsFinished))
            {
                missingResearch = false;
                list.Add(item.label);
            }
        }

        if (!missingResearch)
        {
            Messages.Message(
                "TurretExtensions.UpgradeResearchNotMetMessage".Translate(t.def.label) + ": " +
                list.ToCommaList().CapitalizeFirst(), MessageTypeDefOf.CautionInput, false);
        }
    }

    private IEnumerable<Building_Turret> UpgradableTurretsInSelection(IntVec3 c)
    {
        if (c.Fogged(Map))
        {
            yield break;
        }

        var thingList = c.GetThingList(Map);
        foreach (var item in thingList.Where(t => CanDesignateThing(t).Accepted))
        {
            yield return (Building_Turret)item;
        }
    }
}