using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(TransferableUIUtility), "DoCountAdjustInterfaceInternal")]
    public static class TransferableUIUtility_DoCountAdjustInterfaceInternal_Patch
    {
        public static bool Prefix(Rect rect, Transferable trad, int index, int min, int max, bool flash, bool readOnly)
        {
            if (trad.AnyThing is Pawn pawn && pawn.RaceProps.IsMechanoid && pawn.GetOverseer() is Pawn overseer && overseer.TryGetGestaltEngineInstead(out var comp))
            {
                GUI.color = Color.grey;
                Widgets.Label(rect, "RM.CannotAddGestaltControlled".Translate());
                GUI.color = Color.white;
                return false;
            }
            return true;
        }
    }

}
