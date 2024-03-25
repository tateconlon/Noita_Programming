using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class Room : MonoBehaviour, IComparable<Room>
{
    [Required]
    public Room nextRoom;

    public bool startingRoom = false;
    [FoldoutGroup("Bindings",expanded:false)]
    public RoomTeleporter teleporter;
    [FoldoutGroup("Bindings", expanded:false)]
    public CinemachineVirtualCameraBase virtualCamera;
    // Start is called before the first frame update
    [FoldoutGroup("Bindings", expanded:false)]
    public BoundsWithHandle spawnBoundary;
    [FoldoutGroup("Bindings", expanded: false)]
    public SpellPickup spellPickup1;
    [FoldoutGroup("Bindings", expanded: false)]
    public SpellPickup spellPickup2;

    void Awake()
    {
        teleporter.gameObject.SetActive(false);
        virtualCamera.gameObject.SetActive(startingRoom);
        if (startingRoom)
        {
            EnemySpawner.validSpawnBounds = spawnBoundary;
        }
    }

    void Start()
    {
        GameManager.Instance.Testing.OnSetIsActive += ShowTeleporter;
    }

    private void OnDestroy()
    {
        GameManager.Instance.Testing.OnSetIsActive -= ShowTeleporter;
    }

    void ShowTeleporter(bool isActive)
    {
        //Check that our teleporter is the one close to the player
        if (PlayerControllerV2.instance.gameObject.transform.Distance(teleporter.transform) < 26f)
        {
            if (PlayerControllerV2.instance.gameObject.transform.position.x > 0)
            {
                teleporter.transform.position =
                    PlayerControllerV2.instance.gameObject.transform.position + Vector3.left * 3f;
            }
            else
            {
                teleporter.transform.position =
                    PlayerControllerV2.instance.gameObject.transform.position + Vector3.right * 3f;
            }
        }

        teleporter.gameObject.SetActive(isActive);
    }
    
    public int CompareTo(Room other)
    {
        return string.Compare(name, other.name);  //Culture Specific but this manager temp so who cares
    }
}
