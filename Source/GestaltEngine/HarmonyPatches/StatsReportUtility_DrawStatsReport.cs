using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(StatsReportUtility), nameof(StatsReportUtility.DrawStatsReport), typeof(Rect), typeof(Thing))]
public static class StatsReportUtility_DrawStatsReport
{
    public static void Prefix(Thing thing)
    {
        CompProperties_Power_PowerConsumption.curViewedComp = thing.TryGetComp<CompUpgradeable>();
    }

    public static void Postfix()
    {
        CompProperties_Power_PowerConsumption.curViewedComp = null;
    }
}