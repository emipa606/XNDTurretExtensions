using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    [StaticConstructorOnStartup]
    public static class StaticConstructorClass
    {
        static StaticConstructorClass()
        {
            var allThingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            foreach (var tDef in allThingDefs.Where(tDef =>
                tDef.building != null && tDef.building.IsTurret && (tDef.statBases == null || !tDef.statBases.Any(s => s.stat == StatDefOf.ShootingAccuracyTurret))))
            {
                if (tDef.statBases == null)
                    tDef.statBases = new List<StatModifier>();
                tDef.statBases.Add(new StatModifier {stat = StatDefOf.ShootingAccuracyTurret, value = StatDefOf.ShootingAccuracyTurret.defaultBaseValue});
            }
        }
    }
}