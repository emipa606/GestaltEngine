using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch(typeof(Bill_Mech), "Notify_DoBillStarted")]
    public static class Bill_Mech_Notify_DoBillStarted_Patch
    {
        public static void Postfix(Bill_Mech __instance, Pawn billDoer)
        {
            if (billDoer.RaceProps.IsMechanoid && billDoer.def == GE_DefOf.RM_Mech_Matriarch)
            {
                __instance.boundPawn = billDoer.GetOverseer();
            }
        }
    }
}
