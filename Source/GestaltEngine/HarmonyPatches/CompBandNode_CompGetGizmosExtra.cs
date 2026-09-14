using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CompBandNode), nameof(CompBandNode.CompGetGizmosExtra))]
public static class CompBandNode_CompGetGizmosExtra
{
    private static readonly Texture2D BandNodeTuningIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/BandNodeTuning");

    public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, CompBandNode __instance)
    {
        foreach (var gizmo in __result)
        {
            if (!IsBandNodeTuningCommand(gizmo))
            {
                yield return gizmo;
                continue;
            }

            yield return CreateCommandAction(__instance);
        }
    }

    private static bool IsBandNodeTuningCommand(Gizmo gizmo)
    {
        return gizmo is Command_Action command && command.icon == BandNodeTuningIcon;
    }

    private static Command_Action CreateCommandAction(CompBandNode bandNode)
    {
        var commandAction = new Command_Action
        {
            defaultLabel = bandNode.tunedTo == null
                ? "BandNodeTuneTo".Translate() + "..."
                : "BandNodeRetuneTo".Translate() + "...",
            defaultDesc = bandNode.tunedTo == null
                ? "BandNodeTuningDesc".Translate("PeriodSeconds".Translate(bandNode.Props.tuneSeconds))
                : "BandNodeRetuningDesc".Translate($"{bandNode.Props.retuneDays} " + "Days".Translate()),
            disabled = !HasAvailableMechanitor(bandNode),
            icon = BandNodeTuningIcon
        };

        commandAction.onHover = (Action)Delegate.Combine(commandAction.onHover, (Action)delegate
        {
            var pawn = bandNode.tuningTo ?? bandNode.tunedTo;
            if (pawn == null)
            {
                return;
            }

            GenDraw.DrawLineBetween(
                bandNode.parent.DrawPos,
                pawn.TryGetGestaltEngineInstead(out var comp) ? comp.parent.DrawPos : pawn.DrawPos);
        });

        commandAction.action = (Action)Delegate.Combine(commandAction.action,
            (Action)delegate { Find.WindowStack.Add(new FloatMenu(BuildMechanitorOptions(bandNode))); });

        return commandAction;
    }

    private static bool HasAvailableMechanitor(CompBandNode bandNode)
    {
        if (bandNode.parent.Map.mapPawns.AllPawnsSpawned.Any(pawn => IsSelectableMechanitor(pawn, bandNode)))
        {
            return true;
        }

        return CompGestaltEngine.compGestaltEngines.Any(comp =>
            comp.parent.Map == bandNode.parent.Map && IsSelectableMechanitor(comp.dummyPawn, bandNode));
    }

    private static List<FloatMenuOption> BuildMechanitorOptions(CompBandNode bandNode)
    {
        var options = new List<FloatMenuOption>();

        foreach (var pawn in bandNode.parent.Map.mapPawns.AllPawnsSpawned.Where(pawn =>
                     IsSelectableMechanitor(pawn, bandNode)))
        {
            AddMechanitorOption(options, bandNode, pawn);
        }

        foreach (var comp in CompGestaltEngine.compGestaltEngines)
        {
            if (comp.parent.Map == bandNode.parent.Map && IsSelectableMechanitor(comp.dummyPawn, bandNode))
            {
                AddMechanitorOption(options, bandNode, comp.dummyPawn);
            }
        }

        return options;
    }

    private static bool IsSelectableMechanitor(Pawn pawn, CompBandNode bandNode)
    {
        return MechanitorUtility.IsMechanitor(pawn) && pawn != bandNode.tunedTo;
    }

    private static void AddMechanitorOption(List<FloatMenuOption> options, CompBandNode bandNode, Pawn pawn)
    {
        var localPawn = pawn;
        var label = pawn.Name.ToStringFull;
        label = bandNode.tunedTo != null
            ? $"{label} ({bandNode.RetuneTimeTicks.ToStringTicksToPeriod()})"
            : label + ($" ({bandNode.Props.tuneSeconds} " + "SecondsLower".Translate() + ")");

        options.Add(new FloatMenuOption(label, delegate
        {
            bandNode.tuningTimeLeft = bandNode.tunedTo == null
                ? bandNode.TuningTimeTicks
                : bandNode.RetuneTimeTicks;
            bandNode.tuningTo = localPawn;
        }));
    }
}