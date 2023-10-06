using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(CompPowerTrader), "SetUpPowerVars")]
    public static class CompPowerTrader_SetUpPowerVars_Patch
    {
        public static void Prefix(CompPowerTrader __instance)
        {
            CompProperties_Power_PowerConsumption_Patch.curViewedComp = __instance.parent.TryGetComp<CompUpgradeable>();

        }
        public static void Postfix()
        {
            CompProperties_Power_PowerConsumption_Patch.curViewedComp = null;
        }
    }
}
