using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TurretExtensions;

public class CompUpgradable : ThingComp, IThingHolder, IConstructible
{
    private readonly List<ThingDefCountClass> cachedMaterialsNeeded = [];

    public readonly List<ThingDefCountClass> finalCostList = [];
    private Graphic cachedUpgradedGraphic;

    public ThingOwner innerContainer;

    public bool upgraded;

    public float upgradeWorkDone;

    public float upgradeWorkTotal = -1f;

    public CompProperties_Upgradable Props => (CompProperties_Upgradable)props;

    public Graphic UpgradedGraphic
    {
        get
        {
            if (Props.graphicData != null)
            {
                return cachedUpgradedGraphic ?? (cachedUpgradedGraphic = Props.graphicData.GraphicColoredFor(parent));
            }

            return null;
        }
    }

    public bool SufficientMatsToUpgrade => !(from cost in finalCostList
        let thingCount = innerContainer.TotalStackCountOfDef(cost.thingDef)
        where thingCount < cost.count
        select cost).Any();

    public List<ThingDefCountClass> TotalMaterialCost()
    {
        cachedMaterialsNeeded.Clear();
        foreach (var finalCost in finalCostList)
        {
            var num = innerContainer.TotalStackCountOfDef(finalCost.thingDef);
            var num2 = checked(finalCost.count - num);
            if (num2 > 0)
            {
                cachedMaterialsNeeded.Add(new ThingDefCountClass(finalCost.thingDef, num2));
            }
        }

        return cachedMaterialsNeeded;
    }

    public bool IsCompleted()
    {
        return false;
    }

    public int ThingCountNeeded(ThingDef stuff)
    {
        foreach (var thingDefCountClass in TotalMaterialCost())
        {
            if (thingDefCountClass.thingDef == stuff)
            {
                return thingDefCountClass.count;
            }
        }

        return 0;
    }

    public ThingDef EntityToBuildStuff()
    {
        return parent.Stuff;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public override void PostDeSpawn(Map map)
    {
        base.PostDeSpawn(map);
        map.GetComponent<CompMapTurretExtension>()?.comps.Remove(this);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (innerContainer == null)
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        parent.Map.GetComponent<CompMapTurretExtension>()?.comps.Add(this);
    }

    public override void Initialize(CompProperties props)
    {
        if (props == null)
        {
            Log.Error($"[TE] Received a null props from: {parent.Label}.");
        }

        base.Initialize(props);
        if (innerContainer == null)
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        ResolveCostList();
        if (upgradeWorkTotal == -1f)
        {
            ResolveWorkToUpgrade();
        }
    }

    private void ResolveCostList()
    {
        if (parent.def.MadeFromStuff && Props.costStuffCount > 0)
        {
            finalCostList.Add(new ThingDefCountClass(parent.Stuff, Props.costStuffCount));
        }

        var costList = Props.costList;
        if (costList == null)
        {
            return;
        }

        checked
        {
            foreach (var item in costList)
            {
                var thing1 = item;
                var thingDefCountClass = finalCostList.FirstOrDefault(t => t.thingDef == thing1.thingDef);
                if (thingDefCountClass != null)
                {
                    thingDefCountClass.count += item.count;
                }
                else
                {
                    finalCostList.Add(item);
                }
            }
        }
    }

    private void ResolveWorkToUpgrade()
    {
        if (parent == null || Props == null)
        {
            Log.Warning("[TE] ResolveWorkToUpgrade: parent is null, aborting init.");
            return;
        }

        var num = Props.upgradeWorkFactorStuff && parent.def.MadeFromStuff
            ? parent.Stuff.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.WorkToBuild)
            : 0f;
        var num2 = Props.upgradeWorkFactorStuff && parent.def.MadeFromStuff
            ? parent.Stuff.stuffProps.statFactors.GetStatFactorFromList(StatDefOf.WorkToBuild)
            : 1f;
        upgradeWorkTotal = (Props.workToUpgrade + num) * num2;
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        float num;
        switch (mode)
        {
            case DestroyMode.Vanish:
                return;
            default:
                num = Props.baseResourceDropPct;
                break;
            case DestroyMode.KillFinalize:
                num = Props.destroyedResourceDropPct;
                break;
        }

        var num2 = num;
        foreach (var item in innerContainer)
        {
            item.stackCount = GenMath.RoundRandom(item.stackCount * num2);
            if (item.stackCount == 0)
            {
                item.Destroy();
            }
        }

        innerContainer.TryDropAll(parent.Position, previousMap, ThingPlaceMode.Near);
    }

    public override string CompInspectStringExtra()
    {
        if (ParentHolder is not Map)
        {
            return base.CompInspectStringExtra();
        }

        if (upgraded || parent.Map.designationManager.DesignationOn(parent, DesignationDefOf.UpgradeTurret) == null)
        {
            return null;
        }

        var stringBuilder = new StringBuilder();
        if (!finalCostList.NullOrEmpty())
        {
            stringBuilder.AppendLine($"{"ContainedResources".Translate()}:");
            foreach (var finalCost in finalCostList)
            {
                var thingDef = finalCost.thingDef;
                stringBuilder.AppendLine(
                    $"{thingDef.LabelCap}: {innerContainer.TotalStackCountOfDef(thingDef)} / {finalCost.count}");
            }
        }

        stringBuilder.AppendLine(
            $"{"WorkLeft".Translate()}: {(upgradeWorkTotal - upgradeWorkDone).ToStringWorkAmount()}");
        return stringBuilder.ToString().TrimEndNewlines();
    }

    public void Cancel()
    {
        upgradeWorkDone = 0f;
        innerContainer.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
    }

    public void Upgrade()
    {
        upgraded = true;
        upgradeWorkDone = upgradeWorkTotal;
        if (parent.def.useHitPoints)
        {
            parent.HitPoints = parent.MaxHitPoints;
        }

        if (parent is Building_TurretGun building_TurretGun && Props.turretGunDef != null)
        {
            building_TurretGun.gun.Destroy();
            building_TurretGun.gun = ThingMaker.MakeThing(Props.turretGunDef);
            NonPublicMethods.Building_TurretGun_UpdateGunVerbs(building_TurretGun);
        }

        var compRefuelable = parent.TryGetComp<CompRefuelable>();
        if (compRefuelable != null)
        {
            var num = (float)NonPublicFields.CompRefuelable_fuel.GetValue(compRefuelable) * Props.fuelMultiplierFactor;
            NonPublicFields.CompRefuelable_fuel.SetValue(compRefuelable, num);
        }

        parent.TryGetComp<CompPowerTrader>()?.SetUpPowerVars();
        parent.Map.mapDrawer.SectionAt(parent.Position).RegenerateAllLayers();
    }

    public override string TransformLabel(string label)
    {
        return !upgraded ? label : $"{label} ({"TurretExtensions.TurretUpgradedText".Translate()})";
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        Scribe_Values.Look(ref upgraded, "upgraded");
        Scribe_Values.Look(ref upgradeWorkDone, "upgradeWorkDone");
        Scribe_Values.Look(ref upgradeWorkTotal, "upgradeWorkTotal", -1f);
    }
}