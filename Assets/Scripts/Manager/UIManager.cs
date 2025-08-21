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
    [SerializeField] GameObject temperatureUIPrefab;
    [SerializeField] GameObject buildUIPrefab;
    [SerializeField] GameObject craftUIPrefab;
    [SerializeField] GameObject repairUIPrefab;

    HUD hudUI;
    NPCDialogue npcDialouge;
    InteractionUI interactionUI;
    TemperatureUI temperatureUI;
    BuildUI buildUI;
    CraftUI craftUI;
    RepairUI repairUI;
    UIInventory uiInventory;

    // 인벤토리가 열리면 건축하기UI 안 열리도록, 혹은 그 반대
    // 이걸 어디에 넣어야 할까요...?
    bool isAnyUIOn = false;
    public bool IsAnyUIOn { get { return isAnyUIOn; } set { isAnyUIOn = value; } }
    // enum 방식으로 변경?
    /*public enum ActivedUI
    {
        None = 0,
        Inventory = 1 << 0,
        Build = 1 << 1,
    }*/
    /*private ActivedUI currentUI = ActivedUI.None;
    public ActivedUI CurrentUI => currentUI;*/

    protected override void Awake()
    {
        base.Awake();
        // 헤드업디스플레이 세팅
        hudUI = Instantiate(hudPrefab, gameObject.transform).GetComponent<HUD>();
        npcDialouge = Instantiate(npcDialoguePrefab, gameObject.transform).GetComponent<NPCDialogue>();
        interactionUI = Instantiate(interactionUIPrefab, gameObject.transform).GetComponent<InteractionUI>();
        temperatureUI = Instantiate(temperatureUIPrefab, gameObject.transform).GetComponent<TemperatureUI>();
        uiInventory = Instantiate(inventoryPrefab, gameObject.transform).GetComponent<UIInventory>();
        buildUI = Instantiate(buildUIPrefab, gameObject.transform).GetComponent<BuildUI>();
        craftUI = Instantiate(craftUIPrefab, gameObject.transform).GetComponent <CraftUI>();
        repairUI = Instantiate(repairUIPrefab, gameObject.transform).GetComponent<RepairUI>();
    }
    private void Start()
    {
        InitializeHUD();
        // 인벤토리 연결하는 방법 생각해봐야될듯 어떻게 하지
        buildUI.inventory = uiInventory;
        craftUI.inventory = uiInventory;
        repairUI.inventory = uiInventory;

        npcDialouge.OnDialogueStateChanged += GameManager.Instance.SetPlayerControlActive;
        GameManager.Instance.AddOnInventoryListener(uiInventory.Toggle);
        GameManager.Instance.AddOnBuildListener(buildUI.ToggleBuildUI);

        uiInventory.OnItemConsumed += GameManager.Instance.AddPlayerStatValue; 
        uiInventory.OnEquip += GameManager.Instance.PlayerEquipWeapon;
        uiInventory.UnEquipAction += GameManager.Instance.PlayerUnEquipWeapon;
    }
    private void Update()
    {
        // 테스트
        /*if(Input.GetKeyDown(KeyCode.Tab))
        {
            uiInventory?.Toggle();
        }*/
        // 테스트
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            buildUI?.ToggleBuildUI();
        }*/

        // 테스트
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    craftUI?.ToggleCraftUI();
        //}
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
    public void SetTemperatureUI(float time)
    {
        temperatureUI?.SetTemperature(time);
    }
    public void SetCraftUI()
    {
        craftUI?.ToggleCraftUI();
    }
    public void SetRepairUIText()
    {
        repairUI?.SetText();
    }
    public void DeactivateRepairInteractionUI()
    {
        repairUI?.DeactiveText();
    }
    public void SetRepairWindow(BuildObject buildObject)
    {
        Debug.Log("UI매니저에서 실행");
        repairUI?.OpenRepairWindow(buildObject);
    }
    public void SetBuildTargetObject(BuildObject buildObject)
    {
        repairUI?.SetObjectTarget(buildObject);
    }
}
