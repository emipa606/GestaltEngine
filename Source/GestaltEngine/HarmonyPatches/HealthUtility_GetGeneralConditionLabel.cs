using HarmonyLib;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(HealthUtility), nameof(HealthUtility.GetGeneralConditionLabel))]
public static class HealthUtility_GetGeneralConditionLabel
{
    public static void Postfix(ref string __result, Pawn pawn)
    {
        if (pawn.TryGetGestaltEngineInstead(out _))
        {
            __result = "";
        }
    }
}