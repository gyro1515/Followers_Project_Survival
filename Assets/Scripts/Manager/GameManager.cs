using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    [Header("게임 매니저 세팅")]
    [SerializeField] GameObject dayNightCycleGO;
    List<PlayerCharacter> players = new List<PlayerCharacter>();
    GameObject prefab;
    PlayerController playerController;
    ResourceRespawnController respawnController;
    DayNightCycle dayNightCycle;
    
    protected override void Awake()
    {
        base.Awake();
        // 플레이어 세팅
        prefab = Resources.Load<GameObject>("Prefabs/Player");
        SpawnPlayer();
        playerController = GetPlayer(0)?.GetComponent<PlayerController>();
        SetCursorVisibility(false);
        // 자원, NPC 스폰 컨트롤러 세팅
        prefab = Resources.Load<GameObject>("Prefabs/ResourceRespawn");
        respawnController = Instantiate(prefab, gameObject.transform)?.GetComponent<ResourceRespawnController>();
        // 하루 싸이클 세팅 -> 하이어아키 창에 미리 올려놓은 것과 물 반사가 달라짐
        /*prefab = Resources.Load<GameObject>("Prefabs/DayAndNight");
        dayNightCycle = Instantiate(prefab, gameObject.transform)?.GetComponent<DayNightCycle>();
        dayNightCycle.DayTimeChanged += UIManager.Instance.SetTemperatureUI;
        RenderSettings.sun = dayNightCycle.Sun; // 환경 - 태양 빛 세팅
        // 환경광(Ambient Light), Reflection Probe, Global Illumination 데이터를 다시 계산해서 적용.
        DynamicGI.UpdateEnvironment();*/
        // 따라서 인스펙터 창에 넣은 걸로 스크립트 가져오기 진행
        // null오류 뜨면 Resources/Prefabs/DayAndNight를 게임 매니저 인스펙터창에 넣기
        dayNightCycle = dayNightCycleGO?.GetComponent<DayNightCycle>();
    }
    private void Start()
    {
        dayNightCycle.DayTimeChanged += UIManager.Instance.SetTemperatureUI;
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
