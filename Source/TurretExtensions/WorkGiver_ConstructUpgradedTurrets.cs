using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace TurretExtensions;

[UsedImplicitly]
public class WorkGiver_ConstructUpgradedTurrets : WorkGiver_ConstructDeliverResources
{
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.GetComponent<CompMapTurretExtension>().comps.Select(x => x.parent);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var compUpgradable = t.TryGetComp<CompUpgradable>();
        if (compUpgradable == null)
        {
            return null;
        }

        if (!TurretExtensionsUtility.CanUpgradeTurret(t, pawn, def.workType, compUpgradable, forced))
        {
            return null;
        }

        return ResourceDeliverJobFor(pawn, compUpgradable, false);
    }
}