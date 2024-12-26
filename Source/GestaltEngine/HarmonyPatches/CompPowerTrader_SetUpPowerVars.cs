using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(CompPowerTrader), nameof(CompPowerTrader.SetUpPowerVars))]
public static class CompPowerTrader_SetUpPowerVars
{
    public static void Prefix(CompPowerTrader __instance)
    {
        CompProperties_Power_PowerConsumption.curViewedComp = __instance.parent.TryGetComp<CompUpgradeable>();
    }

    public static void Postfix()
    {
        CompProperties_Power_PowerConsumption.curViewedComp = null;
    }
}