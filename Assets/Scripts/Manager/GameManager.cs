using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    List<PlayerCharacter> players = new List<PlayerCharacter>();
    GameObject prefab;
    protected override void Awake()
    {
        base.Awake();
        prefab = Resources.Load<GameObject>("Prefabs/Player");

        //if(gameObject.IsDestroyed()) return;
        SetCursorVisibility(false);
        SpawnPlayer();
    }
    private void Start()
    {
        
    }
    public void SpawnPlayer()
    {
        PlayerCharacter player = Instantiate(prefab).GetComponent<PlayerCharacter>();
        players.Add(player);
    }
    public PlayerCharacter GetPlayer(int id)
    {
        if (id >= players.Count)
        {
            return null;
        }

        return players[id];
    }

    public void SetCursorVisibility(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetPlayerControlActive(true);
        }
    }
    public void SetPlayerControlActive(bool active)
    {
        GetPlayer(0)?.GetComponent<PlayerController>()?.SetControlActive(active);

    }
}
