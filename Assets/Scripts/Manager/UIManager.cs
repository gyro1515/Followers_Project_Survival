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
    [SerializeField] GameObject buildUIPrefab;
    [SerializeField] GameObject craftUIPrefab;
    private UIInventory uiInventory;
    //public UIInventory UIInventory { get { return uiInventory; } }
    HUD hudUI;
    //public HUD HUDUI { get { return hudUI; } }
    NPCDialogue npcDialouge;
    InteractionUI interactionUI;
    BuildUI buildUI;
    CraftUI craftUI;
    protected override void Awake()
    {
        base.Awake();
        // 헤드업디스플레이 세팅
        hudUI = Instantiate(hudPrefab, gameObject.transform).GetComponent<HUD>();
        npcDialouge = Instantiate(npcDialoguePrefab, gameObject.transform).GetComponent<NPCDialogue>();
        interactionUI = Instantiate(interactionUIPrefab, gameObject.transform).GetComponent<InteractionUI>();
        uiInventory = Instantiate(inventoryPrefab, gameObject.transform).GetComponent<UIInventory>();
        buildUI = Instantiate(buildUIPrefab, gameObject.transform).GetComponent<BuildUI>();
        craftUI = Instantiate(craftUIPrefab, gameObject.transform).GetComponent <CraftUI>();
    }
    private void Start()
    {
        InitializeHUD();
        // 인벤토리 연결하는 방법 생각해봐야될듯 어떻게 하지
        buildUI.inventory = uiInventory;
        craftUI.inventory = uiInventory;
    }
    private void Update()
    {
        // 테스트
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            uiInventory?.Toggle();
        }
        // 테스트
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildUI?.ToggleBuildUI();
        }
        // 테스트
        if (Input.GetKeyDown(KeyCode.C))
        {
            craftUI?.ToggleCraftUI();
        }
    }
    public void InitializeHUD()
    {
        PlayerCharacter player = GameManager.Instance.GetPlayer(0);
        //Debug.Log(player.GetStatComponent<PlayerStatComponent>().statValues.Count);
        if (player.GetStatComponent<PlayerStatComponent>().statValues.Count > 0)
        {
            StatValue statValue;
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Health, out statValue))
            {
                statValue.OnValueChanged += hudUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();
            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Hunger, out statValue))
            {
                statValue.OnValueChanged += hudUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Stamina, out statValue))
            {
                statValue.OnValueChanged += hudUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Thirst, out statValue))
            {
                statValue.OnValueChanged += hudUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
        }
    }
    public void ActiveNPCDialouge()
    {
        npcDialouge?.gameObject.SetActive(true);
    }
    public void SetInteractionUIText(string value) // 세팅하면 자동 활성화
    {
        interactionUI?.SetText(value);
    }
    public void DeactivateInteractionUI()
    {
        interactionUI?.gameObject.SetActive(false);
    }
    public void AddItemToInventory(ItemData itemData)
    {
        uiInventory?.AddItem(itemData);
    }
}
