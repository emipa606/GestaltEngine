using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(Pawn_MechanitorTracker), nameof(Pawn_MechanitorTracker.TotalBandwidth), MethodType.Getter)]
public static class Pawn_MechanitorTracker_TotalBandwidth
{
    public static void Postfix(Pawn_MechanitorTracker __instance, ref int __result)
    {
        if (!__instance.pawn.TryGetGestaltEngineInstead(out var comp))
        {
            return;
        }

        if (comp.compPower?.PowerOn != true)
        {
            __result = 0;
            return;
        }

        __result = comp.CurrentUpgrade?.totalMechBandwidth ?? 0;
        var hediffs = comp.dummyPawn?.health?.hediffSet?.hediffs;
        if (hediffs == null)
        {
            return;
        }

        foreach (var hediff in hediffs)
        {
            var statOffset = hediff.CurStage?.statOffsets?.FirstOrDefault(x => x.stat == StatDefOf.MechBandwidth);
            if (statOffset != null)
            {
                __result += (int)statOffset.value;
            }
        }
    }
}