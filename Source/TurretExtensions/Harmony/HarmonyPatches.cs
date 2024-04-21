using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace TurretExtensions.Harmony;

[StaticConstructorOnStartup]
[UsedImplicitly]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        var harmonyInstance = new HarmonyLib.Harmony("XeoNovaDan.TurretExtensions");
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        var type = ManualPatch_Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate.delegateType =
            typeof(Gizmo_RefuelableFuelStatus).GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic).First();

        harmonyInstance.Patch(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(),
            null, null,
            new HarmonyMethod(typeof(ManualPatch_Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate),
                nameof(ManualPatch_Gizmo_RefuelableFuelStatus.manual_GizmoOnGUI_Delegate.Transpiler)));

        var type2 = ManualPatch_ThingDef.manual_SpecialDisplayStats.enumeratorType = typeof(ThingDef)
            .GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(t => t.Name.Contains("SpecialDisplayStats"));

        harmonyInstance.Patch(
            type2.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m => m.Name == "MoveNext"), null,
            null,
            new HarmonyMethod(typeof(ManualPatch_ThingDef.manual_SpecialDisplayStats),
                nameof(ManualPatch_ThingDef.manual_SpecialDisplayStats.Transpiler)));

        harmonyInstance.Patch(
            typeof(CompRefuelable).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Last(m => m.Name.Contains("CompGetGizmosExtra")), null, null,
            new HarmonyMethod(typeof(ManualPatch_CompRefuelable),
                nameof(ManualPatch_CompRefuelable.FuelCapacityTranspiler)));
    }
}