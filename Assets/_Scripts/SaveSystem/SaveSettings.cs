using ThirteenPixels.Soda;
using UnityEngine;
using ThirteenPixels.Soda.ModuleSettings;

internal class SaveSettings : ModuleSettings
{
    protected override string title => "Save System";

    public Savestate activeSavestate;

    [Header("Events")]
    public GameEvent requestLoadGame;
    public GameEvent requestSaveGame;
}