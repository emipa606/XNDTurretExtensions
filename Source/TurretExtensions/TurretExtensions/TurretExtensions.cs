using HarmonyLib;
using Verse;

namespace TurretExtensions
{
    public class TurretExtensions : Mod
    {
        public static Harmony harmonyInstance;

        public TurretExtensions(ModContentPack content) : base(content)
        {
#if DEBUG
            Log.Error("XeoNovaDan left debugging enabled in Turret Extensions - please let him know!");
#endif

            harmonyInstance = new Harmony("XeoNovaDan.TurretExtensions");
        }
    }
}