﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;

namespace TurretExtensions
{

    public static class HarmonyPatchesUtility
    {

        public static bool IsFuelCapacityInstruction(this CodeInstruction instruction) =>
            instruction.opcode == OpCodes.Ldfld && instruction.operand == AccessTools.Field(typeof(CompProperties_Refuelable), nameof(CompProperties_Refuelable.fuelCapacity));

        public static float AdjustedFuelCapacity(float baseFuelCapacity, Thing t) =>
            baseFuelCapacity * ((t.IsUpgradedTurret(out CompUpgradable uC)) ? uC.Props.effectiveBarrelDurabilityFactor * uC.Props.barrelDurabilityFactor : 1f);

    }

}
