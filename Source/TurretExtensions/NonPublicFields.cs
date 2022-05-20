using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

[StaticConstructorOnStartup]
public static class NonPublicFields
{
    public static FieldInfo CompRefuelable_fuel = AccessTools.Field(typeof(CompRefuelable), "fuel");

    public static FieldInfo DamageDef_externalViolence = AccessTools.Field(typeof(DamageDef), "externalViolence");

    public static FieldInfo DamageDef_externalViolenceForMechanoids =
        AccessTools.Field(typeof(DamageDef), "externalViolenceForMechanoids");

    public static FieldInfo GenDraw_maxRadiusMessaged = AccessTools.Field(typeof(GenDraw), "maxRadiusMessaged");

    public static FieldInfo GenDraw_ringDrawCells = AccessTools.Field(typeof(GenDraw), "ringDrawCells");

    public static FieldInfo StatDrawEntry_overrideReportText =
        AccessTools.Field(typeof(StatDrawEntry), "overrideReportText");
}