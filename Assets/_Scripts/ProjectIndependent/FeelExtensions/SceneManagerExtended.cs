using System.Collections;
using MoreMountains.Tools;

public class SceneManagerExtended : MMAdditiveSceneLoadingManager
{
    /// <summary>
    /// Overrides to only load the anti-spill scene right before the old scenes are unloaded.
    /// This prevents the old scene being rendered with the new lighting settings of the anti-spill scene.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator LoadSequence()
    {
        InitiateLoad();
        yield return ProcessDelayBeforeEntryFade();
        yield return EntryFade();
        yield return ProcessDelayAfterEntryFade();
        _antiSpill?.PrepareAntiFill(_sceneToLoadName);  // Run this right before unloading
        yield return UnloadOriginScenes();
        yield return LoadDestinationScene();
        yield return ProcessDelayBeforeExitFade();
        yield return DestinationSceneActivation();
        yield return ExitFade();
        yield return UnloadSceneLoader();
    }
}
