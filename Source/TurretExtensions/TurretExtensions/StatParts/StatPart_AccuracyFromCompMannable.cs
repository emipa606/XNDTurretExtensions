using RimWorld;
using Verse;

namespace TurretExtensions
{
    public class StatPart_AccuracyFromCompMannable : StatPart
    {
        private readonly StatDef correspondingStat = null;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!ShouldApply(req, out var mannableComp)) return;

            var manningPawn = mannableComp.ManningPawn;

            if (manningPawn == null)
                val = 0;
            else
                val = manningPawn.GetStatValue(correspondingStat);
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!ShouldApply(req, out var mannableComp)) return null;

            var manningPawn = mannableComp.ManningPawn;
            return manningPawn == null
                ? $"{"TurretExtensions.MannableTurretNotManned".Translate()}: {0f.ToStringByStyle(parentStat.toStringStyle, parentStat.toStringNumberSense)}"
                : $"{manningPawn.LabelShortCap}: {manningPawn.GetStatValue(correspondingStat).ToStringByStyle(correspondingStat.toStringStyle, correspondingStat.toStringNumberSense)}";
        }

        private static bool ShouldApply(StatRequest req, out CompMannable mannableComp)
        {
            mannableComp = null;
            if (req.Thing is Building_Turret turret && TurretFrameworkExtension.Get(turret.def).useManningPawnShootingAccuracy)
                mannableComp = turret.GetComp<CompMannable>();

            return mannableComp != null;
        }
    }
}