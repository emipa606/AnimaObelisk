using HarmonyLib;
using PsyObelisk.Things;
using RimWorld;
using UnityEngine;
using Verse;

namespace PsyObelisk.Patches;

[HarmonyPatch(typeof(Pawn_PsychicEntropyTracker))]
[HarmonyPatch("GainPsyfocus")]
public class Pawn_PsychicEntropyTracker_GainPsyfocus_PsyObeliskPatch
{
    private static bool Prefix(Pawn_PsychicEntropyTracker __instance, ref Thing focus)
    {
        if (focus is not { Destroyed: false })
        {
            return true;
        }

        var focus2 = Mathf.Clamp(MeditationUtility.PsyfocusGainPerTick(__instance.Pawn, focus), 0f, 1f);
        var thingComp_PsyObelisk = focus.TryGetComp<ThingComp_PsyObelisk>();
        var compAssignableToPawn_ObeliskGenerator = focus.TryGetComp<CompAssignableToPawn_ObeliskGenerator>();
        if (thingComp_PsyObelisk == null || compAssignableToPawn_ObeliskGenerator == null)
        {
            return true;
        }

        if (!thingComp_PsyObelisk.TryAddFocus(focus2, __instance.Pawn, compAssignableToPawn_ObeliskGenerator))
        {
            return true;
        }

        thingComp_PsyObelisk.GlowAround();
        thingComp_PsyObelisk.GlowPawn(__instance.Pawn);
        return false;
    }
}
