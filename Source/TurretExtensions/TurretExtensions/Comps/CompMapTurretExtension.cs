using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace TurretExtensions
{
    [UsedImplicitly]
    public class CompMapTurretExtension : MapComponent
    {
        public readonly List<CompUpgradable> comps = new List<CompUpgradable>();

        public CompMapTurretExtension(Map map) : base(map) 
        {
        }
    }
}