using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TurretExtensions;

[StaticConstructorOnStartup]
public static class StaticConstructorClass
{
    static StaticConstructorClass()
    {
        foreach (var item in DefDatabase<ThingDef>.AllDefsListForReading.Where(tDef => tDef.building is
                     { IsTurret: true } && (tDef.statBases == null ||
                                            !tDef.statBases.Any(s => s.stat == StatDefOf.ShootingAccuracyTurret))))
        {
            if (item.statBases == null)
            {
                item.statBases = new List<StatModifier>();
            }

            item.statBases.Add(new StatModifier
            {
                stat = StatDefOf.ShootingAccuracyTurret,
                value = StatDefOf.ShootingAccuracyTurret.defaultBaseValue
            });
        }
    }
}