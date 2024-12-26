using HarmonyLib;
using RimWorld;

namespace GestaltEngine;

[HarmonyPatch(typeof(Pawn_MechanitorTracker), nameof(Pawn_MechanitorTracker.TotalAvailableControlGroups),
    MethodType.Getter)]
public static class Pawn_MechanitorTracker_TotalAvailableControlGroups
{
    public static void Postfix(Pawn_MechanitorTracker __instance, ref int __result)
    {
        if (__instance.pawn.TryGetGestaltEngineInstead(out var comp))
        {
            __result = comp.CurrentUpgrade.totalControlGroups;
        }
    }
}