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
        if (hudUI == null)
        {
            hudUI = Instantiate(hudPrefab, uiCanvas).GetComponent<HUD>();

        }
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
