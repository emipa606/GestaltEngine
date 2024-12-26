using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(TransferableUIUtility), nameof(TransferableUIUtility.DrawOverseerIcon))]
public static class TransferableUIUtility_DrawOverseerIcon
{
    public static bool Prefix(Pawn overseer, Rect rect)
    {
        if (!overseer.TryGetGestaltEngineInstead(out var comp))
        {
            return false;
        }

        GUI.DrawTexture(rect, comp.parent.def.uiIcon);
        if (!Mouse.IsOver(rect))
        {
            return false;
        }

        Widgets.DrawHighlight(rect);
        TooltipHandler.TipRegion(rect, "MechOverseer".Translate(overseer));

        return false;
    }
}