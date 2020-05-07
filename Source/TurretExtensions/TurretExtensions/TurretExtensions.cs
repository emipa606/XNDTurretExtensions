using System.Net.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace TurretExtensions
{
    [UsedImplicitly]
    public class TurretExtensions : Mod
    {
        public static Harmony harmonyInstance;

        public TurretExtensions(ModContentPack content) : base(content)
        {
#if DEBUG
            Log.Warning("[TE] Debugging enabled in Turret Extensions - please let the author know!");
#endif

            harmonyInstance = new Harmony("XeoNovaDan.TurretExtensions");
        }
    }
}