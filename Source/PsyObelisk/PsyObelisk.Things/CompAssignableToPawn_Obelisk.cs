using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PsyObelisk.Things;

public class CompAssignableToPawn_Obelisk : CompAssignableToPawn
{
    public override IEnumerable<Pawn> AssigningCandidates
    {
        get
        {
            if (!parent.Spawned)
            {
                return Enumerable.Empty<Pawn>();
            }

            return from x in parent.Map.mapPawns.FreeColonistsSpawned
                where x.HasPsylink
                select x
                into p
                orderby CanAssignTo(p).Accepted descending
                select p;
        }
    }

    public override bool AssignedAnything(Pawn pawn)
    {
        return assignedPawns.Count() >= MaxAssignedPawnsCount;
    }

    public override void TryAssignPawn(Pawn pawn)
    {
        if (assignedPawns.Count() == MaxAssignedPawnsCount)
        {
            assignedPawns.Remove(assignedPawns.Last());
        }

        assignedPawns.Add(pawn);
    }

    public override void TryUnassignPawn(Pawn pawn, bool sort = true, bool uninstall = false)
    {
        assignedPawns.Remove(pawn);
    }

    protected override bool ShouldShowAssignmentGizmo()
    {
        return parent.Faction == Faction.OfPlayer;
    }

    public override AcceptanceReport CanAssignTo(Pawn pawn)
    {
        return !pawn.HasPsylink ? "AnimaObelisk.Messages.PawnHasNotPsylink".Translate() : AcceptanceReport.WasAccepted;
    }

    public override bool IdeoligionForbids(Pawn pawn)
    {
        return false;
    }

    public override void PostExposeData()
    {
        assignedPawns.RemoveAll(x => !x.Spawned || x.Dead);
        base.PostExposeData();
    }
}
