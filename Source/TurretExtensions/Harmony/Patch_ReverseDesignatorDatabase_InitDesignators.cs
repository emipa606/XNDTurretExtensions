using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace TurretExtensions.Harmony;

[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
public static class Patch_ReverseDesignatorDatabase_InitDesignators
{
    public static void Postfix(ref List<Designator> ___desList)
    {
        ___desList.Add(new Designator_UpgradeTurret());
    }
}