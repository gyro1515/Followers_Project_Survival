using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
    // 싱글톤으로 불러가기

    [SerializeField] GameObject inventoryPrefab;
    [SerializeField] GameObject hudPrefab;
    [SerializeField] GameObject npcDialoguePrefab;
    [SerializeField] GameObject interactionUIPrefab;
    private UIInventory uiInventory;
    public UIInventory UIInventory { get { return uiInventory; } }

    HUD hudUI;
    public HUD HUDUI { get { return hudUI; } }

    NPCDialogue npcDialouge;
    InteractionUI interactionUI;
    protected override void Awake()
    {
        base.Awake();
        // 헤드업디스플레이 세팅
        hudUI = Instantiate(hudPrefab, gameObject.transform).GetComponent<HUD>();
        hudUI.SetHpBar(1f); // 실제론 플레이어 체력에 따라 초기 세팅하기
        hudUI.SetSteminaBar(1f);
        hudUI.SetHungerBar(1f);
        hudUI.SetThirstBar(1f);
        uiInventory = Instantiate(inventoryPrefab, gameObject.transform).GetComponent<UIInventory>();
        npcDialouge = Instantiate(npcDialoguePrefab, gameObject.transform).GetComponent<NPCDialogue>();
        interactionUI = Instantiate(interactionUIPrefab, gameObject.transform).GetComponent<InteractionUI>();
    }
    private void Update()
    {
        // 테스트
        if(uiInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            uiInventory.Toggle();
        }
    }
    public void ActiveNPCDialouge()
    {
        npcDialouge.gameObject.SetActive(true);
    }
}
