using RimWorld;
using Verse;

namespace TurretExtensions
{
    public class StatPart_FromMannableTurret : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!ShouldApply(req, out var turret)) return;
            
            var extensionValues = TurretFrameworkExtension.Get(turret.def);
            val += turret.IsUpgraded(out var upgradableComp) ? upgradableComp.Props.manningPawnShootingAccuracyOffset : extensionValues.manningPawnShootingAccuracyOffset;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!ShouldApply(req, out var turret)) return null;
            
            var extensionValues = TurretFrameworkExtension.Get(req.Def);
            var offset = turret.IsUpgraded(out var upgradableComp) ? upgradableComp.Props.manningPawnShootingAccuracyOffset : extensionValues.manningPawnShootingAccuracyOffset;

            return offset != 0 ? $"{turret.def.LabelCap}: {offset.ToStringByStyle(parentStat.ToStringStyleUnfinalized, ToStringNumberSense.Offset)}" : null;
        }

        private bool ShouldApply(StatRequest req, out Building_Turret turret)
        {
            turret = null;
            if (req.Thing is Pawn pawn)
                turret = pawn.MannedThing() as Building_Turret;

            return turret != null;
        }
    }
}