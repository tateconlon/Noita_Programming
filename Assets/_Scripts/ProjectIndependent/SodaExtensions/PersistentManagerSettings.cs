using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda.ModuleSettings;

internal class PersistentManagerSettings : ModuleSettings
{
    protected override string title => "Persistent Managers";

    [Header("Prefabs - Spawned in Listed Order")]
    public List<GameObject> persistentManagerPrefabs = new();
}