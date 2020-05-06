using Verse;

namespace TurretExtensions
{
    public class CompSmartForcedTarget : ThingComp
    {
        public bool attackingNonDownedPawn;

        public CompProperties_SmartForcedTarget Props => (CompProperties_SmartForcedTarget) props;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref attackingNonDownedPawn, "attackingNonDownedPawn");
            base.PostExposeData();
        }
    }
}