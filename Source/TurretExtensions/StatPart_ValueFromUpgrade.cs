using System;
using RimWorld;
using Verse;

namespace TurretExtensions;

public class StatPart_ValueFromUpgrade : StatPart
{
    public override void TransformValue(StatRequest req, ref float val)
    {
        var num = 1;
        try
        {
            if (!req.HasThing || req.Thing.GetInnerIfMinified() is not Building_Turret thing ||
                !thing.IsUpgradable(out var upgradableComp))
            {
                return;
            }

            num = 2;
            if (!upgradableComp.finalCostList.NullOrEmpty())
            {
                num = 3;
                foreach (var item in upgradableComp.innerContainer)
                {
                    num = 4;
                    num = 5;
                    val += item.MarketValue * item.stackCount;
                }
            }

            num = 6;
            val += Math.Min(upgradableComp.upgradeWorkDone, upgradableComp.upgradeWorkTotal) * 0.0036f;
        }
        catch (Exception arg)
        {
            Log.Message($"Exception in getting value from upgrade (failPoint={num}): {arg}");
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        return null;
    }
}