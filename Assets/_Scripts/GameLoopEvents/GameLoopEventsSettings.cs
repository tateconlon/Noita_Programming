using ThirteenPixels.Soda;
using UnityEngine;
using ThirteenPixels.Soda.ModuleSettings;

internal class GameLoopEventsSettings : ModuleSettings
{
    protected override string title => "Game Loop Events";

    [Header("Variables")]
    public GlobalBool isApplicationQuitting;

    [Header("Events")]
    public GameEvent onApplicationQuit;
    
    // TODO: make these statically accessible too
    // TODO: also add a way to check if a GameObject is about to be destroyed due to unloading scene
    // (e.g. can use this in the spawning code to avoid spawning when the scene is about to unload)
}