using System.Linq;
using PsyObelisk.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace PsyObelisk.AI;

public class WorkGiver_PsyObelisk : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest =>
        ThingRequest.ForDef(ThingDefOfLocal.AnimaObelisk_ThingDef_AnimaObelisk);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building building)
        {
            return false;
        }

        if (!pawn.HasPsylink)
        {
            return false;
        }

        var comp = building.GetComp<ThingComp_PsyObelisk>();
        if (comp is not { getFocusActive: true } || comp.focus <= 0.1f)
        {
            return false;
        }

        var comp2 = building.GetComp<CompAssignableToPawn_ObeliskConsumer>();
        if (comp2 == null || !comp2.AssignedPawns.Contains(pawn))
        {
            return false;
        }

        if (pawn.psychicEntropy.CurrentPsyfocus >= pawn.psychicEntropy.TargetPsyfocus - 0.1)
        {
            return false;
        }

        if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
        {
            return false;
        }

        return !t.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building = (Building)t;
        return JobMaker.MakeJob(JobDefOfLocal.AnimaObelisk_JobDef_GetPsyFocus, building);
    }
}
