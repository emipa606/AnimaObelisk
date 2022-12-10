using System;
using Mlie;
using UnityEngine;
using Verse;

namespace PsyObelisk.Settings;

public class AnimaObeliskMod : Mod
{
    private static string currentVersion;
    private readonly Settings settings;

    public AnimaObeliskMod(ModContentPack content)
        : base(content)
    {
        settings = GetSettings<Settings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.Gap(10f);
        listing_Standard.Label("AnimaObelisk.Settings.PsyFocusMax".Translate(settings.psyFocusMax));
        settings.psyFocusMax = (int)listing_Standard.Slider(settings.psyFocusMax, 0.5f, 100f);
        listing_Standard.Gap(10f);
        listing_Standard.Label("AnimaObelisk.Settings.PsyFocusDecayRate".Translate(settings.psyFocusDecayRate));
        settings.psyFocusDecayRate =
            (float)Math.Round(listing_Standard.Slider(settings.psyFocusDecayRate, 0f, 100f), 2);
        listing_Standard.Gap(10f);
        listing_Standard.Label("AnimaObelisk.Settings.PsyFocusGetTime".Translate(settings.psyFocusGetTime));
        settings.psyFocusGetTime = (float)Math.Round(listing_Standard.Slider(settings.psyFocusGetTime, 0f, 30f), 2);
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("AnimaObelisk.Settings.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public override string SettingsCategory()
    {
        return "AnimaObelisk.AnimaObeliskMod".Translate();
    }
}
