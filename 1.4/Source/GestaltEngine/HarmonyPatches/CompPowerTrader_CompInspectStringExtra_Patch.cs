using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(CompPowerTrader), "CompInspectStringExtra")]
    public static class CompPowerTrader_CompInspectStringExtra_Patch
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
