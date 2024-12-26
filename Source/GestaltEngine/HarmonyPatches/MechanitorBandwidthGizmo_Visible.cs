using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(MechanitorBandwidthGizmo), nameof(MechanitorBandwidthGizmo.Visible), MethodType.Getter)]
public static class MechanitorBandwidthGizmo_Visible
{
    public static void Postfix(ref bool __result)
    {
        if (!__result)
        {
            __result = Find.Selector.SelectedObjects
                .OfType<Building>().Count(x => x.GetComp<CompGestaltEngine>() != null) == 1;
        }
    }
}