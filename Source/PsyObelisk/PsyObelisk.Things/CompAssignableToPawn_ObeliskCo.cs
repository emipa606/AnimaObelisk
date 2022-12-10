using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace PsyObelisk.Things;

public class CompAssignableToPawn_ObeliskConsumer : CompAssignableToPawn_Obelisk
{
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        var command_Action = new Command_Action
        {
            defaultLabel = GetAssignmentGizmoLabel(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_FocusReceive"),
            defaultDesc = GetAssignmentGizmoDesc(),
            action = delegate { Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this)); }
        };
        yield return command_Action;
    }

    protected override string GetAssignmentGizmoLabel()
    {
        return "AnimaObelisk.GUI.SetPsycastersToGetPsyfocus_Label".Translate();
    }

    protected override string GetAssignmentGizmoDesc()
    {
        return "AnimaObelisk.GUI.SetPsycastersToGetPsyfocus_Desc".Translate();
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref assignedPawns, "assignedPawnsConsumer", LookMode.Reference);
    }
}
