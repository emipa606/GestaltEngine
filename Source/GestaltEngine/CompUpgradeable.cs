using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace GestaltEngine;

[HotSwappable]
public class CompUpgradeable : ThingComp
{
    private Graphic cachedGraphic;

    public CompPowerTrader compPower;
    private int cooldownPeriod;
    private int downgradeProgressTick = -1;
    public int level;

    private Effecter progressBarEffecter;
    private int upgradeOffset;
    private int upgradeProgressTick = -1;
    private CompProperties_Upgradeable Props => props as CompProperties_Upgradeable;
    private int MinLevel => 0;
    private int MaxLevel => Props.upgrades.Count - 1;
    public Upgrade CurrentUpgrade => Props.upgrades[level];
    private bool Upgrading => upgradeOffset > 0;
    private bool Downgrading => upgradeOffset < 0;
    private bool OnCooldown => cooldownPeriod > Find.TickManager.TicksGame;

    private Graphic OverlayGraphic
    {
        get
        {
            cachedGraphic ??= CurrentUpgrade.overlayGraphic.GraphicColoredFor(parent);

            return cachedGraphic;
        }
    }

    protected virtual bool IsActive => compPower == null || compPower.PowerOn;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        compPower = parent.TryGetComp<CompPowerTrader>();
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        base.PostDeSpawn(map, mode);
        progressBarEffecter?.Cleanup();
    }

    public override void PostDraw()
    {
        base.PostDraw();
        if (CurrentUpgrade.overlayGraphic == null)
        {
            return;
        }

        var vector = parent.DrawPos + Altitudes.AltIncVect;
        vector.y += 5;
        OverlayGraphic.Draw(vector, parent.Rotation, parent);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction != Faction.OfPlayer)
        {
            yield break;
        }

        var upgrade = new Command_Action
        {
            defaultLabel = "RM.Upgrade".Translate(),
            defaultDesc = "RM.UpgradeDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/GestaltUpgrade"),
            action = delegate { startUpgrade(1); }
        };

        var downgrade = new Command_Action
        {
            defaultLabel = "RM.Downgrade".Translate(),
            defaultDesc = "RM.DowngradeDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/GestaltDowngrade"),
            action = delegate { startUpgrade(-1); }
        };

        var upgradeInstant = new Command_Action
        {
            defaultLabel = "DEV: Instant upgrade",
            defaultDesc = "RM.UpgradeDesc".Translate(),
            action = delegate
            {
                startUpgrade(1);
                SetLevel();
            }
        };

        var downgradeInstant = new Command_Action
        {
            defaultLabel = "DEV: Instant downgrade",
            defaultDesc = "RM.DowngradeDesc".Translate(),
            action = delegate
            {
                startUpgrade(-1);
                SetLevel();
            }
        };

        if (!compPower.PowerOn)
        {
            upgrade.Disable("NoPower".Translate());
            downgrade.Disable("NoPower".Translate());
        }
        else if (Upgrading)
        {
            upgrade.Disable("RM.Upgrading".Translate());
            downgrade.Disable("RM.Upgrading".Translate());
        }
        else if (Downgrading)
        {
            upgrade.Disable("RM.Downgrading".Translate());
            downgrade.Disable("RM.Downgrading".Translate());
        }
        else if (OnCooldown)
        {
            upgrade.Disable(
                "RM.OnCooldown".Translate((cooldownPeriod - Find.TickManager.TicksGame).ToStringTicksToPeriod()));
            downgrade.Disable(
                "RM.OnCooldown".Translate((cooldownPeriod - Find.TickManager.TicksGame).ToStringTicksToPeriod()));
        }

        if (level == MinLevel)
        {
            downgrade.disabled = true;
            downgradeInstant.disabled = true;
        }
        else
        {
            downgrade.defaultDesc += $"\n\n{Props.upgrades[level - 1].UpgradeDesc()}";
            downgradeInstant.defaultDesc += $"\n\n{Props.upgrades[level - 1].UpgradeDesc()}";
        }

        if (level == MaxLevel)
        {
            upgrade.disabled = true;
            upgradeInstant.disabled = true;
        }
        else
        {
            upgrade.defaultDesc += $"\n\n{Props.upgrades[level + 1].UpgradeDesc()}";
            upgradeInstant.defaultDesc += $"\n\n{Props.upgrades[level + 1].UpgradeDesc()}";
        }

        yield return upgrade;
        yield return downgrade;
        if (!Prefs.DevMode)
        {
            yield break;
        }

        yield return upgradeInstant;
        yield return downgradeInstant;
    }

    public override string CompInspectStringExtra()
    {
        var sb = new StringBuilder();
        sb.AppendLine("RM.Level".Translate(level));
        if (Upgrading)
        {
            var progress = upgradeProgressTick / (float)CurrentUpgrade.upgradeDurationTicks;
            sb.AppendLine("RM.UpgradeInProcess".Translate(progress.ToStringPercent()));
        }
        else if (Downgrading)
        {
            var progress = downgradeProgressTick / (float)CurrentUpgrade.downgradeDurationTicks;
            sb.AppendLine("RM.DowngradeInProcess".Translate(progress.ToStringPercent()));
        }

        return sb.ToString().TrimEndNewlines();
    }

    private void startUpgrade(int offset)
    {
        SoundDefOf.DragSlider.PlayOneShotOnCamera();
        upgradeOffset = offset;
        switch (offset)
        {
            case < 0:
                downgradeProgressTick = 0;
                break;
            case > 0:
                upgradeProgressTick = 0;
                break;
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        if (CurrentUpgrade.powerConsumption != 0)
        {
            compPower.PowerOutput = -CurrentUpgrade.powerConsumption;
        }

        if (!IsActive)
        {
            return;
        }

        if (Upgrading)
        {
            upgradeProgressTick++;

            progressBarEffecter ??= EffecterDefOf.ProgressBar.Spawn();

            progressBarEffecter.EffectTick(parent, TargetInfo.Invalid);
            var mote = ((SubEffecter_ProgressBar)progressBarEffecter.children[0]).mote;
            mote.progress = upgradeProgressTick / (float)CurrentUpgrade.upgradeDurationTicks;
            mote.offsetZ = -0.8f;

            if (upgradeProgressTick >= CurrentUpgrade.upgradeDurationTicks)
            {
                SetLevel();
            }
        }
        else if (Downgrading)
        {
            downgradeProgressTick++;
            progressBarEffecter ??= EffecterDefOf.ProgressBar.Spawn();

            progressBarEffecter.EffectTick(parent, TargetInfo.Invalid);
            var mote = ((SubEffecter_ProgressBar)progressBarEffecter.children[0]).mote;
            mote.progress = downgradeProgressTick / (float)CurrentUpgrade.downgradeDurationTicks;
            mote.offsetZ = -0.8f;

            if (downgradeProgressTick >= CurrentUpgrade.downgradeDurationTicks)
            {
                SetLevel();
            }
        }

        DoWork();
    }

    protected virtual void DoWork()
    {
        if (!parent.IsHashIntervalTick(60))
        {
            return;
        }

        if (CurrentUpgrade.heatPerSecond != 0)
        {
            GenTemperature.PushHeat(parent.PositionHeld, parent.MapHeld, CurrentUpgrade.heatPerSecond);
        }

        if (CurrentUpgrade.researchPointsPerSecond == 0)
        {
            return;
        }

        var proj = Find.ResearchManager.currentProj;
        if (proj == null)
        {
            return;
        }

        var dictionary = Find.ResearchManager.progress;
        if (dictionary.ContainsKey(proj))
        {
            dictionary[proj] += CurrentUpgrade.researchPointsPerSecond;
        }
        else
        {
            dictionary[proj] = CurrentUpgrade.researchPointsPerSecond;
        }

        if (proj.IsFinished)
        {
            Find.ResearchManager.FinishProject(proj, true);
        }
    }

    protected virtual void SetLevel()
    {
        switch (upgradeOffset)
        {
            case < 0:
                cooldownPeriod = Find.TickManager.TicksGame + CurrentUpgrade.downgradeCooldownTicks;
                Messages.Message("RM.FinishedDowngrade".Translate(parent.LabelCap), parent,
                    MessageTypeDefOf.NeutralEvent);
                break;
            case > 0:
                cooldownPeriod = Find.TickManager.TicksGame + CurrentUpgrade.upgradeCooldownTicks;
                Messages.Message("RM.FinishedUpgrade".Translate(parent.LabelCap), parent,
                    MessageTypeDefOf.NeutralEvent);
                break;
        }

        level += upgradeOffset;
        upgradeOffset = 0;
        downgradeProgressTick = upgradeProgressTick = -1;
        compPower.PowerOutput = -CurrentUpgrade.powerConsumption;
        cachedGraphic = null;
        if (progressBarEffecter == null)
        {
            return;
        }

        progressBarEffecter.Cleanup();
        progressBarEffecter = null;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref level, "level");
        Scribe_Values.Look(ref upgradeOffset, "upgradeOffset");
        Scribe_Values.Look(ref upgradeProgressTick, "upgradeProgressTick", -1);
        Scribe_Values.Look(ref downgradeProgressTick, "downgradeProgressTick", -1);
        Scribe_Values.Look(ref cooldownPeriod, "cooldownPeriod", -1);
    }
}