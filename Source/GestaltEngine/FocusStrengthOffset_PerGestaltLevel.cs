using RimWorld;
using Verse;

namespace GestaltEngine;

public class FocusStrengthOffset_PerGestaltLevel : FocusStrengthOffset
{
    public override string GetExplanation(Thing parent)
    {
        var gestaltComp = parent.TryGetComp<CompGestaltEngine>();
        return "GE.StatsReport_GestaltLevel".Translate(gestaltComp.level) + ": " +
               GetOffset(parent).ToStringWithSign("0%");
    }

    public override string GetExplanationAbstract(ThingDef def = null)
    {
        return "GE.StatsReport_GestaltLevelAbstract".Translate() + ": " + offset.ToStringWithSign("0%");
    }

    public override float GetOffset(Thing parent, Pawn user = null)
    {
        var gestaltComp = parent.TryGetComp<CompGestaltEngine>();
        return offset * (gestaltComp.level + 1);
    }

    public override bool CanApply(Thing parent, Pawn user = null)
    {
        return true;
    }
}