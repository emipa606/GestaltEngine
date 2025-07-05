using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace GestaltEngine;

[HotSwappable]
public class CompGestaltEngine : CompUpgradeable
{
    public static readonly HashSet<CompGestaltEngine> compGestaltEngines = [];
    private Effecter connectMechEffecter;
    private Effecter connectProgressBarEffecter;
    private int connectTick = -1;
    private LocalTargetInfo curTarget = LocalTargetInfo.Invalid;
    public Pawn dummyPawn;
    private int hackCooldownTicks;
    private bool MechanitorActive => compPower.PowerOn && dummyPawn.mechanitor.TotalBandwidth > 0;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        _ = compGestaltEngines.Add(this);
        base.PostSpawnSetup(respawningAfterLoad);
        if (dummyPawn is null)
        {
            dummyPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Mechanitor_Basic, Faction.OfAncients);
            dummyPawn.SetFactionDirect(parent.Faction);
            dummyPawn.Name = new NameSingle(parent.LabelCap);
        }

        PawnComponentsUtility.AddComponentsForSpawn(dummyPawn);
        PawnComponentsUtility.AddAndRemoveDynamicComponents(dummyPawn);
        dummyPawn.mechanitor.Notify_BandwidthChanged();
        dummyPawn.mechanitor.Notify_ControlGroupAmountMayChanged();
        dummyPawn.gender = Gender.None;
        dummyPawn.equipment.DestroyAllEquipment();
        dummyPawn.story.title = "";
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        _ = compGestaltEngines.Remove(this);
        base.PostDeSpawn(map, mode);
    }

    public override void ReceiveCompSignal(string signal)
    {
        switch (signal)
        {
            case "PowerTurnedOn":
            case "PowerTurnedOff":
                dummyPawn.mechanitor.Notify_BandwidthChanged();
                break;
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var g in base.CompGetGizmosExtra())
        {
            yield return g;
        }

        if (parent.Faction != Faction.OfPlayer)
        {
            yield break;
        }

        foreach (var m in dummyPawn.mechanitor.GetGizmos())
        {
            if (m is Command_CallBossgroup)
            {
                continue;
            }

            yield return m;
        }

        var connectMech = new Command_Action
        {
            defaultLabel = "RM.ConnectMechanoid".Translate(),
            defaultDesc = "RM.ConnectMechanoidDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/ConnectMechanoid"),
            action = delegate
            {
                Find.Targeter.BeginTargeting(connectMechanoidTargetParameters(), startConnect, highlight,
                    canConnect);
            }
        };
        var hackMech = new Command_Action
        {
            defaultLabel = "RM.HackMechanoid".Translate(),
            defaultDesc = "RM.HackMechanoidDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/HackMechanoid"),
            action = delegate
            {
                Find.Targeter.BeginTargeting(connectNonColonyMechanoidTargetParameters(), startConnectNonColonyMech,
                    highlight,
                    canConnectNonColonyMech);
            }
        };
        if (curTarget.IsValid)
        {
            connectMech.Disable("RM.BusyConnectingMechanoid".Translate());
            hackMech.Disable("RM.BusyConnectingMechanoid".Translate());
        }
        else if (!MechanitorActive)
        {
            connectMech.Disable("RM.IncapableOfConnectingMechanoid".Translate());
            hackMech.Disable("RM.IncapableOfConnectingMechanoid".Translate());
        }

        if (!hackMech.disabled && hackCooldownTicks > Find.TickManager.TicksGame)
        {
            hackMech.Disable(
                "RM.OnCooldown".Translate((hackCooldownTicks - Find.TickManager.TicksGame)
                    .ToStringTicksToPeriod()));
        }

        yield return connectMech;
        yield return hackMech;
    }

    private TargetingParameters connectMechanoidTargetParameters()
    {
        return new TargetingParameters
        {
            canTargetPawns = true,
            canTargetBuildings = false,
            canTargetHumans = false,
            canTargetMechs = true,
            canTargetAnimals = false,
            canTargetLocations = false,
            validator = x => canConnect((LocalTargetInfo)x)
        };
    }

    private TargetingParameters connectNonColonyMechanoidTargetParameters()
    {
        return new TargetingParameters
        {
            canTargetPawns = true,
            canTargetBuildings = false,
            canTargetHumans = false,
            canTargetMechs = true,
            canTargetAnimals = false,
            canTargetLocations = false,
            validator = x => canConnectNonColonyMech((LocalTargetInfo)x)
        };
    }

    private static int mechControlTime(Pawn mech)
    {
        return Mathf.RoundToInt(mech.GetStatValue(StatDefOf.ControlTakingTime) * 60f);
    }

    private bool canConnect(LocalTargetInfo target)
    {
        var mech = target.Pawn;
        return mech != null && canControlMech(dummyPawn, mech);
    }

    private bool canConnectNonColonyMech(LocalTargetInfo target)
    {
        var mech = target.Pawn;
        return mech != null && canControlNonColonyMech(dummyPawn, mech) && hasEnoughBandwidth(dummyPawn, mech)
               && mech.Faction != parent.Faction;
    }

    private static AcceptanceReport canControlMech(Pawn pawn, Pawn mech)
    {
        if (pawn.mechanitor == null || !mech.IsColonyMech || mech.Downed || mech.Dead || mech.IsAttacking())
        {
            return false;
        }

        if (!MechanitorUtility.EverControllable(mech))
        {
            return "CannotControlMechNeverControllable".Translate();
        }

        if (mech.GetOverseer() == pawn)
        {
            return "CannotControlMechAlreadyControlled".Translate(pawn.LabelShort);
        }

        return true;
    }

    private static AcceptanceReport canControlNonColonyMech(Pawn pawn, Pawn mech)
    {
        if (pawn.mechanitor == null || mech.Downed || mech.Dead)
        {
            return false;
        }

        if (!MechanitorUtility.EverControllable(mech))
        {
            return "CannotControlMechNeverControllable".Translate();
        }

        if (mech.GetOverseer() == pawn)
        {
            return "CannotControlMechAlreadyControlled".Translate(pawn.LabelShort);
        }

        return true;
    }

    private static bool hasEnoughBandwidth(Pawn pawn, Pawn mech)
    {
        var num = pawn.mechanitor.TotalBandwidth - pawn.mechanitor.UsedBandwidth;
        var statValue = mech.GetStatValue(StatDefOf.BandwidthCost);
        return !(num < statValue);
    }

    private void startConnect(LocalTargetInfo target)
    {
        curTarget = target;
        connectTick = Find.TickManager.TicksGame + mechControlTime(curTarget.Pawn);
        PawnUtility.ForceWait(curTarget.Pawn, mechControlTime(curTarget.Pawn), parent, true);
        if (!hasEnoughBandwidth(dummyPawn, curTarget.Pawn))
        {
            Messages.Message("RM.NotEnoughBandwidth".Translate(), curTarget.Pawn, MessageTypeDefOf.CautionInput);
        }
    }

    private void startConnectNonColonyMech(LocalTargetInfo target)
    {
        curTarget = target;
        var connectPeriod = mechControlTime(curTarget.Pawn) * 2;
        connectTick = Find.TickManager.TicksGame + connectPeriod;
    }

    protected override void SetLevel()
    {
        base.SetLevel();
        dummyPawn.mechanitor.Notify_BandwidthChanged();
        dummyPawn.mechanitor.Notify_ControlGroupAmountMayChanged();
    }

    public override void CompTick()
    {
        base.CompTick();
        if (dummyPawn.Faction != parent.Faction)
        {
            dummyPawn.SetFaction(parent.Faction);
        }

        if (curTarget.IsValid && connectTick != -1)
        {
            var mech = curTarget.Pawn;
            if (mech.Faction == dummyPawn.Faction && !canConnect(mech)
                || mech.Faction != dummyPawn.Faction && !canConnectNonColonyMech(mech)
                || !MechanitorActive)
            {
                reset();
            }
            else
            {
                connectEffects(mech);
                if (Find.TickManager.TicksGame >= connectTick)
                {
                    connect(curTarget, dummyPawn);
                }
            }
        }

        if (!dummyPawn.IsHashIntervalTick(60))
        {
            return;
        }

        foreach (var hediff in dummyPawn.health.hediffSet.hediffs.OfType<Hediff_BandNode>())
        {
            hediff.RecacheBandNodes();
        }
    }

    private void connectEffects(Pawn mech)
    {
        connectProgressBarEffecter ??= EffecterDefOf.ProgressBar.Spawn();
        connectProgressBarEffecter.EffectTick(parent, TargetInfo.Invalid);
        var mote = ((SubEffecter_ProgressBar)connectProgressBarEffecter.children[0]).mote;
        mote.progress = 1f - ((connectTick - Find.TickManager.TicksGame) / (mech.Faction != parent.Faction
            ? mechControlTime(mech) * 2f
            : mechControlTime(mech)));
        mote.offsetZ = -0.8f;

        if (connectMechEffecter == null)
        {
            connectMechEffecter = EffecterDefOf.ControlMech.Spawn();
            connectMechEffecter.Trigger(parent, mech);
        }

        connectMechEffecter.EffectTick(parent, mech);
    }

    private void connect(LocalTargetInfo target, Pawn pawn)
    {
        reset();
        var mech = target.Pawn;
        if (mech.Faction != dummyPawn.Faction)
        {
            mech.SetFaction(dummyPawn.Faction);
            hackCooldownTicks = Find.TickManager.TicksGame + (GenDate.TicksPerDay * 5);
        }

        mech.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, mech);
        pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mech);
    }

    private void reset()
    {
        connectProgressBarEffecter.Cleanup();
        connectProgressBarEffecter = null;
        connectMechEffecter.Cleanup();
        connectMechEffecter = null;
        if (curTarget.Thing is Pawn { Dead: false } pawn)
        {
            pawn.jobs.StopAll();
            pawn.pather.StopDead();
        }

        curTarget = LocalTargetInfo.Invalid;
        connectTick = -1;
    }

    private static void highlight(LocalTargetInfo target)
    {
        if (target.IsValid)
        {
            GenDraw.DrawTargetHighlight(target);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref dummyPawn, "dummyPawn");
        Scribe_TargetInfo.Look(ref curTarget, "CompGestaltEngine_curTarget", LocalTargetInfo.Invalid);
        Scribe_Values.Look(ref connectTick, "CompGestaltEngine_connectTick", -1);
        Scribe_Values.Look(ref hackCooldownTicks, "CompGestaltEngine_hackCooldownTicks");
    }
}