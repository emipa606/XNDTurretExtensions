using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretExtensions
{
    public class CompUpgradable : ThingComp, IThingHolder, IConstructible
    {
        private Graphic cachedUpgradedGraphic;
        
        private readonly List<ThingDefCountClass> cachedMaterialsNeeded = new List<ThingDefCountClass>();
        public readonly List<ThingDefCountClass> finalCostList = new List<ThingDefCountClass>();
        
        public ThingOwner innerContainer;
        public bool upgraded;
        public float upgradeWorkDone;
        public float upgradeWorkTotal = -1;
        public CompProperties_Upgradable Props => (CompProperties_Upgradable) props;
        
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            var compMap = map.GetComponent<CompMapTurretExtension>();
            compMap?.comps.Remove(this);
#if DEBUG
            Log.Message($"[TE] CompList contains {parent.Map.GetComponent<CompMapTurretExtension>().comps.Count} entries.");
#endif
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            
            if (innerContainer == null)
            {
                innerContainer = new ThingOwner<Thing>(this, false);
#if DEBUG
                Log.Warning("[TE] Encountered an invalid Turret inventory (innerContainer), rebuilding inventory.");
#endif
            }
            
            var compMap = parent.Map.GetComponent<CompMapTurretExtension>();
            compMap?.comps.Add(this);
#if DEBUG
            Log.Message($"[TE] CompList contains {parent.Map.GetComponent<CompMapTurretExtension>().comps.Count} entries.");
#endif
        }

        public Graphic UpgradedGraphic
        {
            get
            {
                if (Props.graphicData != null) return cachedUpgradedGraphic ?? (cachedUpgradedGraphic = Props.graphicData.GraphicColoredFor(parent));

                return null;
            }
        }

#if DEBUG 
        public bool SufficientMatsToUpgrade
        {
            get
            {
                foreach (var cost in finalCostList)
                {
                    if (cost?.thingDef == null)
                    {
                        Log.Error("[TE] ThingDef in CostList returned null.");
                        return false;
                    }
                    
                    if (innerContainer == null)
                    {
                        Log.Error("[TE] Turret inventory (innerContainer) does not exist, how did this happen?");
                        return false;
                    }

                    if (innerContainer.Any(x => x == null))
                    {
                        Log.Error("[TE] Turret inventory (innerContainer) contains null reference.");
                        return false;
                    }
                    
                    var thingCount = innerContainer.TotalStackCountOfDef(cost.thingDef);
                    if (thingCount < cost.count) return false;
                }

                return true;
            }
        }
#else
        public bool SufficientMatsToUpgrade => !(from cost in finalCostList let thingCount = innerContainer.TotalStackCountOfDef(cost.thingDef) where thingCount < cost.count select cost).Any();
#endif

        public List<ThingDefCountClass> MaterialsNeeded()
        {
            cachedMaterialsNeeded.Clear();

            // Determine needed materials
            foreach (var cost in finalCostList)
            {
                var resourceCount = innerContainer.TotalStackCountOfDef(cost.thingDef);
                var amountNeeded = cost.count - resourceCount;
                if (amountNeeded > 0)
                    cachedMaterialsNeeded.Add(new ThingDefCountClass(cost.thingDef, amountNeeded));
            }

            return cachedMaterialsNeeded;
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

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            innerContainer = new ThingOwner<Thing>(this, false);
            
            ResolveCostList();
            if (upgradeWorkTotal == -1)
                ResolveWorkToUpgrade();
        }

        private void ResolveCostList()
        {
            if (parent.def.MadeFromStuff && Props.costStuffCount > 0)
                finalCostList.Add(new ThingDefCountClass(parent.Stuff, Props.costStuffCount));
            var costList = Props.costList;
            if (costList == null) return;

            foreach (var thing in costList)
            {
                var thing1 = thing;
                var duplicate = finalCostList.FirstOrDefault(t => t.thingDef == thing1.thingDef);
                if (duplicate != null)
                    duplicate.count += thing.count;
                else
                    finalCostList.Add(thing);
            }
        }

        private void ResolveWorkToUpgrade()
        {
            var upgradeWorkOffset = Props.upgradeWorkFactorStuff && parent.def.MadeFromStuff
                ? parent.Stuff.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.WorkToBuild)
                : 0f;
            var upgradeWorkFactor = Props.upgradeWorkFactorStuff && parent.def.MadeFromStuff
                ? parent.Stuff.stuffProps.statFactors.GetStatFactorFromList(StatDefOf.WorkToBuild)
                : 1f;

            upgradeWorkTotal = (Props.workToUpgrade + upgradeWorkOffset) * upgradeWorkFactor;
        }


        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // If the turret wasn't minified, drop anything inside the innerContainer
            if (mode == DestroyMode.Vanish) return;

            var resourceDropFraction = mode == DestroyMode.KillFinalize ? Props.destroyedResourceDropPct : Props.baseResourceDropPct;

            foreach (var thing in innerContainer)
            {
                thing.stackCount = GenMath.RoundRandom(thing.stackCount * resourceDropFraction);
                if (thing.stackCount == 0)
                    thing.Destroy();
            }

            innerContainer.TryDropAll(parent.Position, previousMap, ThingPlaceMode.Near);
        }

        public override string CompInspectStringExtra()
        {
            if (!(ParentHolder is Map))
                return base.CompInspectStringExtra();

            // Not upgraded but designated to be upgraded
            if (upgraded || parent.Map.designationManager.DesignationOn(parent, DesignationDefOf.UpgradeTurret) == null) return null;

            var inspectBuilder = new StringBuilder();

            // Resource costs
            if (!finalCostList.NullOrEmpty())
            {
                inspectBuilder.AppendLine($"{"ContainedResources".Translate()}:");
                foreach (var cost in finalCostList)
                {
                    var costDef = cost.thingDef;
                    inspectBuilder.AppendLine($"{costDef.LabelCap}: {innerContainer.TotalStackCountOfDef(costDef)} / {cost.count}");
                }
            }

            // Work left
            inspectBuilder.AppendLine($"{"WorkLeft".Translate()}: {(upgradeWorkTotal - upgradeWorkDone).ToStringWorkAmount()}");

            return inspectBuilder.ToString().TrimEndNewlines();
        }

        public void Cancel()
        {
            upgradeWorkDone = 0;
            innerContainer.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
        }

        public void Upgrade()
        {
            upgraded = true;
            upgradeWorkDone = upgradeWorkTotal;

            // Set health to max health
            if (parent.def.useHitPoints)
                parent.HitPoints = parent.MaxHitPoints;

            // Update turret top
            if (parent is Building_TurretGun gunTurret && Props.turretGunDef != null)
            {
                gunTurret.gun.Destroy();
                gunTurret.gun = ThingMaker.MakeThing(Props.turretGunDef);
                NonPublicMethods.Building_TurretGun_UpdateGunVerbs(gunTurret);
            }

            // Update barrel durability
            if (parent.TryGetComp<CompRefuelable>() is CompRefuelable refuelableComp)
            {
                var newFuel = (float) NonPublicFields.CompRefuelable_fuel.GetValue(refuelableComp) * Props.fuelMultiplierFactor;
                NonPublicFields.CompRefuelable_fuel.SetValue(refuelableComp, newFuel);
            }

            // Reset CompPowerTrader
            if (parent.TryGetComp<CompPowerTrader>() is CompPowerTrader powerComp)
                powerComp.SetUpPowerVars();

            // Force redraw
            parent.Map.mapDrawer.SectionAt(parent.Position).RegenerateAllLayers();
        }

        public override string TransformLabel(string label)
        {
            return upgraded ? $"{label} ({"TurretExtensions.TurretUpgradedText".Translate()})" : label;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Values.Look(ref upgraded, "upgraded");
            Scribe_Values.Look(ref upgradeWorkDone, "upgradeWorkDone");
            Scribe_Values.Look(ref upgradeWorkTotal, "upgradeWorkTotal", -1);
        }
    }
}