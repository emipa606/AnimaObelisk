using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace PsyObelisk.Things;

public class CompAssignableToPawn_ObeliskGenerator : CompAssignableToPawn_Obelisk
{
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        var command_Action = new Command_Action
        {
            defaultLabel = GetAssignmentGizmoLabel(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/AnimaObelisk_FocusStore"),
            defaultDesc = GetAssignmentGizmoDesc(),
            action = delegate { Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this)); }
        };
        yield return command_Action;
    }

    protected override string GetAssignmentGizmoLabel()
    {
        return "AnimaObelisk.GUI.SetPsycastersToMeditation_Label".Translate();
    }

    protected override string GetAssignmentGizmoDesc()
    {
        return "AnimaObelisk.GUI.SetPsycastersToMeditation_Desc".Translate();
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref assignedPawns, "assignedPawnsGenerator", LookMode.Reference);
    }
}
