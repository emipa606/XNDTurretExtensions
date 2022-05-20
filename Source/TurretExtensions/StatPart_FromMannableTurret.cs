using RimWorld;
using Verse;

namespace TurretExtensions;

public class StatPart_FromMannableTurret : StatPart
{
    public override void TransformValue(StatRequest req, ref float val)
    {
        if (!ShouldApply(req, out var turret))
        {
            return;
        }

        var turretFrameworkExtension = TurretFrameworkExtension.Get(turret.def);
        val += turret.IsUpgraded(out var upgradableComp)
            ? upgradableComp.Props.manningPawnShootingAccuracyOffset
            : turretFrameworkExtension.manningPawnShootingAccuracyOffset;
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (!ShouldApply(req, out var turret))
        {
            return null;
        }

        var turretFrameworkExtension = TurretFrameworkExtension.Get(req.Def);
        var num = turret.IsUpgraded(out var upgradableComp)
            ? upgradableComp.Props.manningPawnShootingAccuracyOffset
            : turretFrameworkExtension.manningPawnShootingAccuracyOffset;
        if (num == 0f)
        {
            return null;
        }

        return
            $"{turret.def.LabelCap}: {num.ToStringByStyle(parentStat.ToStringStyleUnfinalized, ToStringNumberSense.Offset)}";
    }

    private static bool ShouldApply(StatRequest req, out Building_Turret turret)
    {
        turret = null;
        if (req.Thing is Pawn pawn)
        {
            turret = pawn.MannedThing() as Building_Turret;
        }

        return turret != null;
    }
}