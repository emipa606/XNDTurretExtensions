using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace TurretExtensions;

[UsedImplicitly]
public class CompMapTurretExtension(Map map) : MapComponent(map)
{
    public readonly List<CompUpgradable> comps = [];
}