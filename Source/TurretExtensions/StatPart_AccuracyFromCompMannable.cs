using RimWorld;
using Verse;

namespace TurretExtensions;

public class StatPart_AccuracyFromCompMannable : StatPart
{
    private readonly StatDef correspondingStat;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (!ShouldApply(req, out var mannableComp))
        {
            return;
        }

        var manningPawn = mannableComp.ManningPawn;
        val = manningPawn?.GetStatValue(correspondingStat) ?? 0f;
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (!ShouldApply(req, out var mannableComp))
        {
            return null;
        }

        var manningPawn = mannableComp.ManningPawn;
        if (manningPawn != null)
        {
            return
                $"{manningPawn.LabelShortCap}: {manningPawn.GetStatValue(correspondingStat).ToStringByStyle(correspondingStat.toStringStyle, correspondingStat.toStringNumberSense)}";
        }

        return string.Format("{0}: {1}", "TurretExtensions.MannableTurretNotManned".Translate(),
            0f.ToStringByStyle(parentStat.toStringStyle, parentStat.toStringNumberSense));
    }

    private static bool ShouldApply(StatRequest req, out CompMannable mannableComp)
    {
        mannableComp = null;
        if (req.Thing is Building_Turret building_Turret &&
            TurretFrameworkExtension.Get(building_Turret.def).useManningPawnShootingAccuracy)
        {
            mannableComp = building_Turret.GetComp<CompMannable>();
        }

        return mannableComp != null;
    }
}