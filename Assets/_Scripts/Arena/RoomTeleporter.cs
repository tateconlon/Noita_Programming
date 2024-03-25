using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;

    [SerializeField] 
    private AudioClip teleportSfx;
    [Required, SerializeField, FoldoutGroup("Bindings", expanded:false)]
    private Room _myRoom;

    public static event Action TeleportEvent; //Quick n' dirty way to turn off all teleporters when you've teleported
    void Start()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        if (cinemachineBrain == null)
        {
            Debug.LogError("Cinemachine Brain not found on the main camera.", gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Teleport(col.gameObject);
        }
    }

    void Teleport(GameObject player)
    {
        player.transform.position = _myRoom.nextRoom.teleporter.transform.position - Vector3.up * 6f; //Temp Positional offset so you don't spawn on a teleporter
        _myRoom.nextRoom.virtualCamera.gameObject.SetActive(true);
        EnemySpawner.validSpawnBounds = _myRoom.nextRoom.spawnBoundary;
        TeleportEvent?.Invoke();
        M.am.PlayGlobalSfx(teleportSfx);
    }
}
