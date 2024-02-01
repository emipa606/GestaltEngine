using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(StatsReportUtility), "DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) })]
    public static class StatsReportUtility_DrawStatsReport_Patch
    {
        public static void Prefix(Thing thing)
        {
            CompProperties_Power_PowerConsumption_Patch.curViewedComp = thing.TryGetComp<CompUpgradeable>();
        }

        public static void Postfix()
        {
            CompProperties_Power_PowerConsumption_Patch.curViewedComp = null;
        }
    }
}
