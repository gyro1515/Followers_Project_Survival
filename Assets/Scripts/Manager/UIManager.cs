using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
    // 싱글톤으로 불러가기

    [SerializeField] Transform uiCanvas;
    [SerializeField] GameObject inventoryPrefab;
    [SerializeField] GameObject hudPrefab;
    private UIInventory uiInventory;
    public UIInventory UIInventory { get { return uiInventory; } }
    HUD hudUI;
    public HUD HUDUI { get { return hudUI; } }
    protected override void Awake()
    {
        base.Awake();
        uiInventory = Instantiate(inventoryPrefab, uiCanvas).GetComponent<UIInventory>();
        // 헤드업디스플레이 세팅
        hudUI = Instantiate(hudPrefab, uiCanvas).GetComponent<HUD>();
        hudUI.SetHpBar(1f); // 실제론 플레이어 체력에 따라 초기 세팅하기
        hudUI.SetSteminaBar(1f);
        hudUI.SetHungerBar(1f);
        hudUI.SetThirstBar(1f);
    }
    private void Update()
    {
        // 테스트
        if(uiInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            uiInventory.Toggle();
        }
    }
}
