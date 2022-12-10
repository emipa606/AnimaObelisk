using PsyObelisk.Settings;
using Verse;

namespace PsyObelisk;

public static class Utilities
{
    public static Settings.Settings Settings =>
        LoadedModManager.GetMod<AnimaObeliskMod>().GetSettings<Settings.Settings>();
}
