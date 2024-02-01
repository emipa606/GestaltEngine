using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(Bill_Mech), "PawnAllowedToStartAnew")]
    public static class Bill_Mech_PawnAllowedToStartAnew_Patch
    {
        public static void Prefix(ref Pawn p)
        {
            if (p.RaceProps.IsMechanoid && p.def == GE_DefOf.RM_Mech_Matriarch && p.GetOverseer() != null)
            {
                p = p.GetOverseer();
            }
        }
    }
}
