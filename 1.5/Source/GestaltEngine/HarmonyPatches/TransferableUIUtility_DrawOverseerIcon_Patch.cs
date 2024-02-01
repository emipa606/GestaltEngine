using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(TransferableUIUtility), "DrawOverseerIcon")]
    public static class TransferableUIUtility_DrawOverseerIcon_Patch
    {
        public static bool Prefix(Pawn mech, Pawn overseer, Rect rect)
        {
            if (overseer.TryGetGestaltEngineInstead(out var comp))
            {
                GUI.DrawTexture(rect, comp.parent.def.uiIcon);
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                    TooltipHandler.TipRegion(rect, "MechOverseer".Translate(overseer));
                }
                return false;
            }
            return false;
        }
    }

}
