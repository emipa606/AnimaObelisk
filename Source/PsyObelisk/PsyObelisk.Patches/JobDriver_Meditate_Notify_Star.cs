using System.Linq;
using HarmonyLib;
using PsyObelisk.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace PsyObelisk.Patches;

[HarmonyPatch(typeof(JobDriver_Meditate))]
[HarmonyPatch("Notify_Starting")]
public class JobDriver_Meditate_Notify_Starting_PsyObeliskPatch
{
    private static bool Prefix(ref JobDriver_Meditate __instance)
    {
        var pawn = __instance.pawn;
        var list = __instance.pawn.Map.listerBuildings.allBuildingsColonist.Where(x =>
                x.def == ThingDefOfLocal.AnimaObelisk_ThingDef_AnimaObelisk &&
                x.GetComp<ThingComp_PsyObelisk>() != null)
            .ToList();
        if (list.NullOrEmpty())
        {
            return true;
        }

        var building = list.Find(delegate(Building x)
        {
            var comp2 = x.GetComp<ThingComp_PsyObelisk>();
            return comp2 is { getFocusActive: true, focus: >= 0.1f } &&
                   x.GetComp<CompAssignableToPawn_ObeliskConsumer>().AssignedPawns.Contains(pawn) &&
                   pawn.CanReserveAndReach(x, PathEndMode.ClosestTouch, Danger.None);
        });
        if (building != null)
        {
            pawn.jobs.ClearQueuedJobs();
            pawn.jobs.TryTakeOrderedJob(new Job(JobDefOfLocal.AnimaObelisk_JobDef_GetPsyFocus, building), JobTag.Misc,
                true);
            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            return false;
        }

        var source = list.Where(delegate(Building x)
        {
            var comp = x.GetComp<ThingComp_PsyObelisk>();
            return comp is { meditationActive: true } &&
                   x.GetComp<CompAssignableToPawn_ObeliskGenerator>().AssignedPawns.Contains(pawn) &&
                   pawn.CanReach(x, PathEndMode.Touch, Danger.None);
        }).ToList();
        source.TryRandomElement(out var result);
        if (result == null)
        {
            return true;
        }

        var curJob = pawn.jobs.curJob;
        if (curJob.targetC.Thing != null && source.Contains(curJob.targetC.Thing))
        {
            return false;
        }

        (from x in GenRadial.RadialCellsAround(result.Position, 5f, false)
            where pawn.CanReach(x, PathEndMode.OnCell, Danger.None)
            select x).TryRandomElement(out var result2);
        pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.Meditate, result2, null, result), JobTag.Misc);
        return false;
    }
}
