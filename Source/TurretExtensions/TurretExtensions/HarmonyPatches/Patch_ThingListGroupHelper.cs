using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace TurretExtensions
{
    [HarmonyPatch(typeof(ThingListGroupHelper), nameof(ThingListGroupHelper.Includes))]
    public class Includes
    {
        public static bool Prefix(ref ThingRequestGroup group, ref ThingDef def, ref bool __result)
        {
            if (def == null || __result) return true;
            if (group != ThingRequestGroup.Blueprint || !def.HasComp(typeof(CompUpgradable))) return true;
            
            __result = true;
            return false;
        }
    }
}