using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

[StaticConstructorOnStartup]
public static class NonPublicFields
{
    public static readonly FieldInfo CompRefuelable_fuel = AccessTools.Field(typeof(CompRefuelable), "fuel");

    public static readonly FieldInfo DamageDef_externalViolence =
        AccessTools.Field(typeof(DamageDef), "externalViolence");

    public static readonly FieldInfo DamageDef_externalViolenceForMechanoids =
        AccessTools.Field(typeof(DamageDef), "externalViolenceForMechanoids");

    public static readonly FieldInfo
        GenDraw_maxRadiusMessaged = AccessTools.Field(typeof(GenDraw), "maxRadiusMessaged");

    public static readonly FieldInfo GenDraw_ringDrawCells = AccessTools.Field(typeof(GenDraw), "ringDrawCells");

    public static FieldInfo StatDrawEntry_overrideReportText =
        AccessTools.Field(typeof(StatDrawEntry), "overrideReportText");
}