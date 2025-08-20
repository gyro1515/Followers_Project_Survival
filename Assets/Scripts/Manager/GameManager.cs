using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    List<PlayerCharacter> players = new List<PlayerCharacter>();
    GameObject prefab;
    PlayerController playerController;
    protected override void Awake()
    {
        base.Awake();
        prefab = Resources.Load<GameObject>("Prefabs/Player");

        //if(gameObject.IsDestroyed()) return;
        SpawnPlayer();
        playerController = GetPlayer(0)?.GetComponent<PlayerController>();
        SetCursorVisibility(false);
        
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
            SetPlayerControlActive(false);
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
        playerController?.SetControlActive(active);
    }
    /*public void SetPlayerControlActiveTogle()
    {
        playerController?.SetControlActiveTogle();
    }*/

    public void AddOnInventoryListener(Action listener)
    {
        playerController.OnInventotyAction += listener;
    }
    public void AddOnBuildListener(Action listener)
    {
        playerController.OnBuildAction += listener;
    }
}
