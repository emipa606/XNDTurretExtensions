using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace TurretExtensions;

public static class Patch_ReverseDesignatorDatabase
{
    [HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
    public static class InitDesignators
    {
        public static void Postfix(ReverseDesignatorDatabase __instance, ref List<Designator> ___desList)
        {
            ___desList.Add(new Designator_UpgradeTurret());
        }
    }
}