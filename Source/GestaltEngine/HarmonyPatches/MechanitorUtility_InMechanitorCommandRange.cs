using HarmonyLib;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(MechanitorUtility), nameof(MechanitorUtility.InMechanitorCommandRange))]
public static class MechanitorUtility_InMechanitorCommandRange
{
    public static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)
    {
        var overseer = mech.GetOverseer();
        if (overseer != null && overseer.TryGetGestaltEngineInstead(out _))
        {
            __result = true;
        }
    }
}