﻿using HarmonyLib;
using RimWorld;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(CompProperties_Power), "PowerConsumption", MethodType.Getter)]
    public static class CompProperties_Power_PowerConsumption_Patch
    {
        public static CompUpgradeable curViewedComp;
        public static void Postfix(ref float __result)
        {
            if (curViewedComp != null)
            {
                __result = curViewedComp.CurrentUpgrade.powerConsumption;
            }
        }
    }
}
