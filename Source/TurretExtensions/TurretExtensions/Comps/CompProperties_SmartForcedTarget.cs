using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace TurretExtensions
{
    [UsedImplicitly]
    public class CompProperties_SmartForcedTarget : CompProperties
    {
        public bool onlyApplyWhenUpgraded;

        public CompProperties_SmartForcedTarget()
        {
            compClass = typeof(CompSmartForcedTarget);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var e in base.ConfigErrors(parentDef))
                yield return e;

            if (!onlyApplyWhenUpgraded || parentDef.HasComp(typeof(CompUpgradable))) yield break;

            yield return "has onlyApplyWhenUpgraded set to true but doesn't have CompUpgradable";

            onlyApplyWhenUpgraded = false;
        }
    }
}