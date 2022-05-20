using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace TurretExtensions;

[UsedImplicitly]
public class TurretExtensions : Mod
{
    public static Harmony harmonyInstance;

    public TurretExtensions(ModContentPack content)
        : base(content)
    {
        harmonyInstance = new Harmony("XeoNovaDan.TurretExtensions");
    }
}