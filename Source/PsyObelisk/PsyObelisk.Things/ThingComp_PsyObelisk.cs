using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Random = UnityEngine.Random;

namespace PsyObelisk.Things;

public class ThingComp_PsyObelisk : ThingComp
{
    public float focus;

    public bool getFocusActive = true;

    public int lastMeditationTick;

    public bool meditationActive = true;

    public CompProperties_PsyObelisk Props => props as CompProperties_PsyObelisk;

    public float FocusMax => Utilities.Settings.psyFocusMax;

    public int Tick => Find.TickManager.TicksGame;

    public override void CompTick()
    {
        MainGlow();
        if (Tick % 60 == 0 && lastMeditationTick + 60 <= Tick)
        {
            focus = Mathf.Clamp(focus - (Utilities.Settings.psyFocusDecayRate / 100000f), 0f, FocusMax);
        }
    }

    public void MainGlow()
    {
        if (focus <= 0.01)
        {
            return;
        }

        var x = focus / FocusMax * 100f;
        var simpleCurve = new SimpleCurve(new List<CurvePoint>
        {
            new CurvePoint(0f, 70f),
            new CurvePoint(100f, 20f)
        });
        if (Tick % (int)simpleCurve.Evaluate(x) == 0)
        {
            MoteMaker.MakeStaticMote(parent.TrueCenter(), parent.Map,
                ThingDefOfLocal.AnimaObelisk_ThingDef_RunesGlow, 7f);
        }
    }

    public void GlowAround()
    {
        if (Tick % 30 != 0 || FocusMax - focus < 0.01f)
        {
            return;
        }

        var num = Rand.Range(1.85f, 2.25f);
        var vector = parent.TrueCenter();
        for (var i = 0; i < 3; i++)
        {
            var vector2 = vector + (new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f).normalized * num);
            vector2.x = Mathf.Clamp(vector2.x, 0f, parent.Map.Size.x);
            vector2.z = Mathf.Clamp(vector2.z, 0f, parent.Map.Size.z);
            MoteMaker.MakeStaticMote(vector2, parent.Map, ThingDefOfLocal.AnimaObelisk_ThingDef_SimpleGlow,
                Rand.Range(0.3f, 0.85f));
        }
    }

    public void GlowPawn(Pawn pawn)
    {
        if (Tick % 60 == 0 && !(FocusMax - focus < 0.01f))
        {
            MoteMaker.MakeStaticMote(pawn.Position, parent.Map, ThingDefOfLocal.AnimaObelisk_ThingDef_EffectToPawn,
                Rand.Range(1.3f, 1.85f));
        }
    }

    public float Focus(Pawn pawn)
    {
        if (!(1f - pawn.psychicEntropy.CurrentPsyfocus > focus))
        {
            return 1f - pawn.psychicEntropy.CurrentPsyfocus;
        }

        return focus;
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        var list = base.CompFloatMenuOptions(selPawn).ToList();
        list.AddRange(base.CompFloatMenuOptions(selPawn));
        var floatMenuOption = new FloatMenuOption("AnimaObelisk.FloatMenuOption.CompletelyFillPawn".Translate(),
            delegate
            {
                var job = new Job(JobDefOfLocal.AnimaObelisk_JobDef_GetPsyFocus, parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            })
        {
            Disabled = !selPawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.None) || !selPawn.HasPsylink ||
                       selPawn.psychicEntropy.CurrentPsyfocus >= selPawn.psychicEntropy.TargetPsyfocus || focus <= 0.01
        };
        list.Add(floatMenuOption);
        if (!selPawn.HasPsylink)
        {
            floatMenuOption.Label +=
                " " + "AnimaObelisk.FloatMenuOption.CompletelyFillPawnFailReasonHasNotPsylink".Translate();
            return list;
        }

        if (selPawn.psychicEntropy.CurrentPsyfocus >= selPawn.psychicEntropy.TargetPsyfocus)
        {
            floatMenuOption.Label +=
                " " + "AnimaObelisk.FloatMenuOption.CompletelyFillPawnFailReasonFullPawn".Translate();
            return list;
        }

        if (!(focus <= 0.01))
        {
            return list;
        }

        floatMenuOption.Label +=
            " " + "AnimaObelisk.FloatMenuOption.CompletelyFillPawnFailReasonEmpryStorage".Translate();
        return list;
    }

    public bool TryAddFocus(float focusToAdd, Pawn pawn, CompAssignableToPawn_ObeliskGenerator comp)
    {
        if (!meditationActive || !comp.AssignedPawns.Contains(pawn) || focus + focusToAdd >= FocusMax)
        {
            return false;
        }

        focus = Mathf.Clamp(focus + focusToAdd, 0f, FocusMax);
        lastMeditationTick = Tick;
        return true;
    }

    public override string CompInspectStringExtra()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.CompInspectStringExtra());
        stringBuilder.AppendLineIfNotEmpty();
        stringBuilder.AppendLine("AnimaObelisk.GUI.FocusLevel".Translate(Math.Round(focus, 3), FocusMax));
        stringBuilder.AppendLine("AnimaObelisk.GUI.MeditationActive".Translate(meditationActive.ToString()));
        stringBuilder.Append("AnimaObelisk.GUI.GetFocusActive".Translate(getFocusActive.ToString()));
        return stringBuilder.ToString();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        if (Prefs.DevMode)
        {
            var commandAction100 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (100%)",
                action = delegate { focus = FocusMax; }
            };
            yield return commandAction100;
            var commandAction80 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (80%)",
                action = delegate { focus = FocusMax / 100f * 80f; }
            };
            yield return commandAction80;
            var commandAction60 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (60%)",
                action = delegate { focus = FocusMax / 100f * 60f; }
            };
            yield return commandAction60;
            var commandAction40 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (40%)",
                action = delegate { focus = FocusMax / 100f * 40f; }
            };
            yield return commandAction40;
            var commandAction20 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (20%)",
                action = delegate { focus = FocusMax / 100f * 20f; }
            };
            yield return commandAction20;
            var commandAction0 = new Command_Action
            {
                defaultLabel = "Debug: fill obelisk (0%)",
                action = delegate { focus = 0f; }
            };
            yield return commandAction0;
        }

        var commandActionConsume = new Command_Action
        {
            defaultLabel = "AnimaObelisk.GUI.GetFocusActiveSwitch_Label".Translate(),
            defaultDesc = "AnimaObelisk.GUI.GetFocusActiveSwitch_Desc".Translate(),
            icon = getFocusActive
                ? ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_ModeConsume")
                : ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_ModeConsumeDisable"),
            action = delegate { getFocusActive = !getFocusActive; }
        };
        yield return commandActionConsume;
        var commandActionSwitch = new Command_Action
        {
            defaultLabel = "AnimaObelisk.GUI.MeditationActiveSwitch_Label".Translate(),
            defaultDesc = "AnimaObelisk.GUI.MeditationActiveSwitch_Desc".Translate(),
            icon = meditationActive
                ? ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_ModeMeditate")
                : ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_ModeMeditateDisable"),
            action = delegate { meditationActive = !meditationActive; }
        };
        yield return commandActionSwitch;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref focus, "focus");
        Scribe_Values.Look(ref getFocusActive, "getFocusActive");
        Scribe_Values.Look(ref meditationActive, "meditationActive");
        Scribe_Values.Look(ref lastMeditationTick, "lastMeditationTick");
    }
}
