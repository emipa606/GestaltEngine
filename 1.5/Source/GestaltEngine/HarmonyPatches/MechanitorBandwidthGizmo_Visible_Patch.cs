using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(MechanitorBandwidthGizmo), "Visible", MethodType.Getter)]
    public static class MechanitorBandwidthGizmo_Visible_Patch
    {
        public static void Postfix(ref bool __result)
        {
            if (!__result)
            {
                __result = Find.Selector.SelectedObjects.OfType<Building>().Where(x => x.GetComp<CompGestaltEngine>() != null).Count() == 1;
            }
        }
    }
}
