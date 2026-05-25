using System.Linq;
using HarmonyLib;
using RimWorld;

namespace GestaltEngine;

[HarmonyPatch(typeof(GameEnder), nameof(GameEnder.CheckOrUpdateGameOver))]
public static class GameEnder_CheckOrUpdateGameOver
{
    public static void Postfix(GameEnder __instance)
    {
        if (!__instance.gameEnding)
        {
            return;
        }

        if (CompGestaltEngine.compGestaltEngines.Any(comp => comp.parent?.Faction == Faction.OfPlayer))
        {
            __instance.gameEnding = false;
        }
    }
}