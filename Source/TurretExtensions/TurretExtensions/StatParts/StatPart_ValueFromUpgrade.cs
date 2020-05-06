using System;
using RimWorld;
using Verse;

namespace TurretExtensions
{
    public class StatPart_ValueFromUpgrade : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            var failPoint = 1;
            try
            {
                if (!req.HasThing || !(req.Thing.GetInnerIfMinified() is Building_Turret turret) || !turret.IsUpgradable(out var uC)) return;
                
                failPoint = 2;
                if (!uC.finalCostList.NullOrEmpty())
                {
                    failPoint = 3;
                    foreach (var t in uC.innerContainer)
                    {
                        failPoint = 4;
                        var thing = t;
                        failPoint = 5;
                        val += thing.MarketValue * thing.stackCount;
                    }
                }

                failPoint = 6;
                val += Math.Min(uC.upgradeWorkDone, uC.upgradeWorkTotal) * StatWorker_MarketValue.ValuePerWork;
            }
            catch (Exception ex)
            {
                Log.Message($"Exception in getting value from upgrade (failPoint={failPoint}): {ex}");
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            return null;
        }
    }
}