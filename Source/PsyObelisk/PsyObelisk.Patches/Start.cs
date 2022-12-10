using HarmonyLib;
using Verse;

namespace PsyObelisk.Patches;

[StaticConstructorOnStartup]
public static class Start
{
    static Start()
    {
        new Harmony("DimonSever000.PsyObelisk").PatchAll();
        Log.Message("Psy obelisk harmony patches loaded successfully");
    }
}
