using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace GestaltEngine;

[HarmonyPatch(typeof(Bill), nameof(Bill.PawnAllowedToStartAnew))]
public static class Bill_PawnAllowedToStartAnew
{
    public static bool Prefix(Bill __instance, Pawn p, ref bool __result)
    {
        if (!__instance.recipe.mechanitorOnlyRecipe || !p.RaceProps.IsMechanoid || p.def != GE_DefOf.RM_Mech_Matriarch)
        {
            return true;
        }

        if (__instance.PawnRestriction != null)
        {
            __result = __instance.PawnRestriction == p;
            return false;
        }

        if (__instance.SlavesOnly && !p.IsSlave)
        {
            __result = false;
            return false;
        }

        if (__instance.MechsOnly && !p.IsColonyMechPlayerControlled)
        {
            __result = false;
            return false;
        }

        if (__instance.NonMechsOnly && p.IsColonyMechPlayerControlled)
        {
            __result = false;
            return false;
        }

        if (__instance.recipe.workSkill != null && (p.skills != null || p.IsColonyMech))
        {
            var num = p.skills != null ? p.skills.GetSkill(__instance.recipe.workSkill).Level : p.RaceProps.mechFixedSkillLevel;
            if (num < __instance.allowedSkillRange.min)
            {
                JobFailReason.Is("UnderAllowedSkill".Translate(__instance.allowedSkillRange.min), __instance.Label);
                __result = false;
                return false;
            }

            if (num > __instance.allowedSkillRange.max)
            {
                JobFailReason.Is("AboveAllowedSkill".Translate(__instance.allowedSkillRange.max), __instance.Label);
                __result = false;
                return false;
            }
        }

        var overseer = p.GetOverseer();
        if (overseer == null || !MechanitorUtility.IsMechanitor(overseer))
        {
            JobFailReason.Is("NotAMechanitor".Translate());
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }
}
