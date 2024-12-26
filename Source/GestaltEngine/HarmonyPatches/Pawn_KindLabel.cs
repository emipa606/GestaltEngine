using HarmonyLib;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.KindLabel), MethodType.Getter)]
public static class Pawn_KindLabel
{
    public static void Postfix(Pawn __instance, ref string __result)
    {
        if (__instance.TryGetGestaltEngineInstead(out _))
        {
            __result = "";
        }
    }
}