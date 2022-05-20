using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretExtensions;

[StaticConstructorOnStartup]
public static class NonPublicProperties
{
    public static Action<TurretTop, float> TurretTop_set_CurRotation =
        (Action<TurretTop, float>)Delegate.CreateDelegate(typeof(Action<TurretTop, float>), null,
            AccessTools.Property(typeof(TurretTop), "CurRotation").GetSetMethod(true));
}