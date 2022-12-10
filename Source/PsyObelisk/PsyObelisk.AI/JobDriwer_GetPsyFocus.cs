using System.Collections.Generic;
using PsyObelisk.Things;
using Verse;
using Verse.AI;

namespace PsyObelisk.AI;

public class JobDriwer_GetPsyFocus : JobDriver
{
    private int FillDuration => (int)(Utilities.Settings.psyFocusGetTime * 60f);

    protected Building Obelisk => job.GetTarget(TargetIndex.A).Thing as Building;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Obelisk, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        AddEndCondition(() => JobCondition.Ongoing);
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        yield return Toils_General.Wait(FillDuration).FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                var thingComp_PsyObelisk = Obelisk.TryGetComp<ThingComp_PsyObelisk>();
                if (thingComp_PsyObelisk == null)
                {
                    return;
                }

                var num = thingComp_PsyObelisk.Focus(pawn);
                pawn.psychicEntropy.OffsetPsyfocusDirectly(num);
                thingComp_PsyObelisk.focus -= num;
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}
