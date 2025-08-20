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
    private UIInventory uiInventory;
    //public UIInventory UIInventory { get { return uiInventory; } }
    HUD hudUI;
    //public HUD HUDUI { get { return hudUI; } }
    NPCDialogue npcDialouge;
    InteractionUI interactionUI;
    TemperatureUI temperatureUI;
    BuildUI buildUI;
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
    }
    private void Start()
    {
        InitializeHUD();
        buildUI.inventory = uiInventory;
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
            buildUI.ToggleBuildUI();
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
    public void SetTemperatureUI(float time)
    {
        temperatureUI?.SetTemperature(time);
    }
}
