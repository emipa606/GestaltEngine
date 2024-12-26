using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(TransferableUIUtility), nameof(TransferableUIUtility.DoCountAdjustInterfaceInternal))]
public static class TransferableUIUtility_DoCountAdjustInterfaceInternal
{
    public static bool Prefix(Rect rect, Transferable trad, int index, int min, int max, bool flash, bool readOnly)
    {
        if (trad.AnyThing is not Pawn pawn || !pawn.RaceProps.IsMechanoid || pawn.GetOverseer() is not { } overseer ||
            !overseer.TryGetGestaltEngineInstead(out _))
        {
            return true;
        }

        GUI.color = Color.grey;
        Widgets.Label(rect, "RM.CannotAddGestaltControlled".Translate());
        GUI.color = Color.white;
        return false;
    }
}