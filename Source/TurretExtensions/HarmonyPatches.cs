using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace TurretExtensions;

[StaticConstructorOnStartup]
[UsedImplicitly]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        TurretExtensions.harmonyInstance.PatchAll();
        var type = Patch_Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate.delegateType =
            typeof(Gizmo_RefuelableFuelStatus).GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic).First();
        TurretExtensions.harmonyInstance.Patch(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(),
            null, null,
            new HarmonyMethod(typeof(Patch_Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate), "Transpiler"));
        var type2 = Patch_ThingDef.manual_SpecialDisplayStats.enumeratorType = typeof(ThingDef)
            .GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(t => t.Name.Contains("SpecialDisplayStats"));
        TurretExtensions.harmonyInstance.Patch(
            type2.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m => m.Name == "MoveNext"), null,
            null, new HarmonyMethod(typeof(Patch_ThingDef.manual_SpecialDisplayStats), "Transpiler"));
        TurretExtensions.harmonyInstance.Patch(
            typeof(CompRefuelable).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Last(m => m.Name.Contains("CompGetGizmosExtra")), null, null,
            new HarmonyMethod(typeof(Patch_CompRefuelable), "FuelCapacityTranspiler"));
    }
}