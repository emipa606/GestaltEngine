using System;
using HarmonyLib;
using RimWorld;
using Verse;

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
            return;
        }

        if (__instance.pawn.Name.ToStringFull == DefDatabase<ThingDef>.GetNamed("RM_GestaltEngine").label)
        {
            __result = Math.Max(10, __result);
        }
    }
}