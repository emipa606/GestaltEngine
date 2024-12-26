using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GestaltEngine;

public class Upgrade
{
    public float heatPerSecond;
    public GraphicData overlayGraphic;
    public float powerConsumption;
    public float researchPointsPerSecond;
    public int totalControlGroups;
    public int totalMechBandwidth;
    public List<RecipeDef> unlockedRecipes = [];
    public int upgradeCooldownTicks, downgradeCooldownTicks = -1;
    public int upgradeDurationTicks, downgradeDurationTicks = -1;

    public string UpgradeDesc()
    {
        var sb = new StringBuilder();
        if (powerConsumption != 0)
        {
            sb.AppendLine("PowerConsumption".Translate() + ": " + powerConsumption.ToString("F0") + " W");
        }

        if (heatPerSecond != 0)
        {
            sb.AppendLine("GE.HeatPerSecond".Translate(heatPerSecond.ToStringDecimalIfSmall()));
        }

        if (researchPointsPerSecond != 0)
        {
            sb.AppendLine("GE.ResearchPointsPerSecond".Translate(researchPointsPerSecond.ToStringDecimalIfSmall()));
        }

        if (totalMechBandwidth != 0)
        {
            sb.AppendLine("GE.TotalMechBandwidth".Translate(totalMechBandwidth));
        }

        if (totalControlGroups != 0)
        {
            sb.AppendLine("GE.TotalControlGroups".Translate(totalControlGroups));
        }

        if (unlockedRecipes.NullOrEmpty() is false)
        {
            sb.AppendLine("GE.UnlocksRecipes".Translate(string.Join(", ", unlockedRecipes.Select(x => x.label))));
        }

        return sb.ToString().TrimEndNewlines();
    }
}