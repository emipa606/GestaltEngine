using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[HarmonyPatch(typeof(MechanitorUtility), nameof(MechanitorUtility.GetMechGizmos))]
public static class MechanitorUtility_GetMechGizmos
{
    public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn mech)
    {
        foreach (var gizmo in __result)
        {
            if (gizmo is Command_Action { disabled: true } command &&
                command.defaultLabel == "CommandSelectOverseer".Translate())
            {
                var overseer = mech.GetOverseer();
                if (overseer != null && overseer.TryGetGestaltEngineInstead(out var comp))
                {
                    command.defaultDesc = "CommandSelectOverseerDesc".Translate();
                    command.disabled = false;
                    command.icon = ContentFinder<Texture2D>.Get("UI/Buttons/GestaltOverseer");
                    command.action = delegate
                    {
                        Find.Selector.ClearSelection();
                        Find.Selector.Select(comp.parent);
                    };
                }
            }

            yield return gizmo;
        }
    }
}