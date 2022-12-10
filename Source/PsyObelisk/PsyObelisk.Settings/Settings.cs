using Verse;

namespace PsyObelisk.Settings;

public class Settings : ModSettings
{
    public float psyFocusDecayRate = 5f;

    public float psyFocusGetTime = 10f;
    public int psyFocusMax = 10;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref psyFocusMax, "psyFocusMax", 10);
        Scribe_Values.Look(ref psyFocusDecayRate, "psyFocusDecayRate", 5f);
        Scribe_Values.Look(ref psyFocusGetTime, "psyFocusGetTime", 10f);
    }
}
