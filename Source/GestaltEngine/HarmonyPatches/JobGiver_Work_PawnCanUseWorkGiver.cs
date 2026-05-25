using HarmonyLib;
using RimWorld;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(JobGiver_Work), "PawnCanUseWorkGiver")]
public static class JobGiver_Work_PawnCanUseWorkGiver
{
    public static void Postfix(Pawn pawn, WorkGiver giver, ref bool __result)
    {
        if (__result || !pawn.RaceProps.IsMechanoid || pawn.def != GE_DefOf.RM_Mech_Matriarch)
        {
            return;
        }

        if (giver?.def?.defName != "DoBillsSubcoreEncoder")
        {
            return;
        }

        if (!giver.def.nonColonistsCanDo && !pawn.IsColonist && !pawn.IsColonyMech && !pawn.IsColonySubhuman)
        {
            return;
        }

        if (pawn.WorkTagIsDisabled(giver.def.workTags))
        {
            return;
        }

        if (giver.def.workType != null && pawn.WorkTypeIsDisabled(giver.def.workType))
        {
            return;
        }

        if (giver.ShouldSkip(pawn))
        {
            return;
        }

        if (giver.MissingRequiredCapacity(pawn) != null)
        {
            return;
        }

        __result = true;
    }
}
