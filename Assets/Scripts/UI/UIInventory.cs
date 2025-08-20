using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] ItemSlot[] slots;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] Transform slotPanel;
    Transform dropPosition;      // item 버릴 때 필요한 위치

    [Header("Selected Item")]           // 선택한 슬롯의 아이템 정보 표시 위한 UI
    [SerializeField] TextMeshProUGUI selectedItemName;
    [SerializeField] TextMeshProUGUI selectedItemDescription;
    [SerializeField] TextMeshProUGUI selectedItemStatName;
    [SerializeField] TextMeshProUGUI selectedItemStatValue;
    [SerializeField] GameObject useButton;
    [SerializeField] GameObject equipButton;
    [SerializeField] GameObject unEquipButton;
    [SerializeField] GameObject dropButton;
    [Header("오디오 클립")]
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip openCloseClip;

    private ItemSlot selectedItem;
    private int selectedItemIndex;

    private int curEquipIndex;

    AudioSource audioSource;

    // 임시. 나중에 바꿔야 함
    public ItemSlot[] Slots { get { return slots; } }

    private void Awake()
    {
        // Inventory UI 초기화 로직들
        // 원하는 개수만큼 소환하는 방식으로 바꿔도 됨

        slots = new ItemSlot[slotPanel.childCount]; // 자식 개수 가져오기

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].Init(this, i);
            slots[i].Clear();
        }

        ClearSelectedItemWindow();

        // 버튼 클릭 할당
        useButton.GetComponent<Button>().onClick.AddListener(OnUseButton);
        dropButton.GetComponent<Button>().onClick.AddListener(OnDropButton);
        equipButton.GetComponent<Button>().onClick.AddListener(OnEquipButton);
        unEquipButton.GetComponent<Button>().onClick.AddListener(OnUnEquipButton);

        audioSource = gameObject.GetComponent<AudioSource>();

        // ***********플레이어 기준 버리는 위치를 가져와야 하지만 현재는 플레이어가 없으므로 카메라로 대체
        dropPosition = Camera.main.transform;
    }
    private void Start()
    {

        Toggle(); // 시작 시 Inventory 창 닫기
    }
    // 선택한 아이템 표시할 정보창 Clear 함수
    void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemIndex = -1;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // Inventory 창 Open/Close 시 호출
    public void Toggle()
    {
        if (IsOpen()) inventoryWindow.SetActive(false);
        else inventoryWindow.SetActive(true);
        if (openCloseClip) audioSource.PlayOneShot(openCloseClip);
    }
    public bool IsOpen()
    {
        return inventoryWindow.activeSelf;
    }
    public void AddItem(ItemData data, int quantity = 1)
    {
        // 여러개 가질 수 있는 아이템이라면

        // enum을 사용하는게 더 좋지 않나...
        // if(data.Type == EItemType.Resource)
        // 어차피 형변환을 한 번 해야 한다면 이렇게 해도...?
        StackableItemData stackableData = data as StackableItemData;
        ItemSlot emptySlot;
        if (stackableData != null)
        {
            //ItemSlot slot = GetItemStack(stackableData);
            //if (slot != null)
            //{
            //    slot.Quantity++;
            //    UpdateUI();
            //    return;
            //}

            /////////////////////////

            while (quantity > 0)
            {
                ItemSlot slot = GetItemStack(stackableData);
                if (slot != null)
                {
                    int cap = stackableData.MaxStackAmount - slot.Quantity;
                    int add = Mathf.Min(quantity, cap);
                    slot.Quantity += add;
                    quantity -= add;
                }
                else
                {
                    emptySlot = GetEmptySlot();

                    if (emptySlot != null)
                    {
                        emptySlot.Item = data;
                        int num = Mathf.Min(quantity, stackableData.MaxStackAmount);
                        emptySlot.Quantity = num;
                        quantity -= num;
                    }
                }

                UpdateUI();
            }

            return;
        }

        // 빈 슬롯 찾기
        emptySlot = GetEmptySlot();

        // 빈 슬롯이 있다면
        if (emptySlot != null)
        {
            emptySlot.Item = data;
            emptySlot.Quantity = 1;
            UpdateUI();
            return;
        }

        // 빈 슬롯 마저 없을 때
        ThrowItem(data);
    }

    // UI 정보 새로고침
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 슬롯에 아이템 정보가 있다면
            if (slots[i].Item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    // 여러개 가질 수 있는 아이템의 정보 찾아서 return
    ItemSlot GetItemStack(StackableItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item == data && slots[i].Quantity < data.MaxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 슬롯의 item 정보가 비어있는 정보 return
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 아이템 버리기 (실제론 매개변수로 들어온 데이터에 해당하는 아이템 생성)
    public void ThrowItem(ItemData data)
    {
        Instantiate(data.DropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }


    // 선택한 아이템 정보창에 업데이트 해주는 함수
    public void SelectItem(int index)
    {
        if (slots[index].Item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.Item.DisplayName;
        selectedItemDescription.text = selectedItem.Item.Description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        // 소비 아이템 이라면
        ConsumableItemData consumableItemData = selectedItem.Item as ConsumableItemData;
        if (consumableItemData != null)
        {
            for (int i = 0; i < consumableItemData.Consumables.Length; i++)
            {
                selectedItemStatName.text += consumableItemData.Consumables[i].type.ToString() + "\n";
                selectedItemStatValue.text += consumableItemData.Consumables[i].value.ToString() + "\n";
            }
        }
        

        useButton.SetActive(selectedItem.Item.ItemType == EItemType.Consumable);
        //equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        //unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(!slots[index].equipped); // 장착되면 버리지 못하도록
    }

    public void OnUseButton()
    {
        if (selectedItem.Item.ItemType == EItemType.Consumable)
        {
            //stateController.ApplyConsumable(selectedItem.item);
            
            RemoveSelctedItem();
        }
        if (clickClip) audioSource.PlayOneShot(clickClip);

    }

    public void OnDropButton()
    {
        if (selectedItem.equipped) return; // 장착된 아이템은 버리지 못하도록
        ThrowItem(selectedItem.Item);
        RemoveSelctedItem();
        if (clickClip) audioSource.PlayOneShot(clickClip);
    }

    void RemoveSelctedItem()
    {
        selectedItem.Quantity--;

        if (selectedItem.Quantity <= 0)
        {
            if (slots[selectedItemIndex].equipped)
            {
                //UnEquip(selectedItemIndex);
            }

            selectedItem.Item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    
    public void OnEquipButton()
    {
        //if (selectedItemIndex == curEquipIndex) return;

        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        //GameManager.Instance.Player.equip.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
        if (clickClip) audioSource.PlayOneShot(clickClip);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        //GameManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
        if (clickClip) audioSource.PlayOneShot(clickClip);
    }

    public int GetTotalQuantity(ItemData item)  // 인벤토리 전체에 해당 아이템이 몇 개있는지 반환
    {
        int total = 0;
        foreach(var slot in slots)
        {
            if(slot.Item == item)
            {
                total += slot.Quantity;
            }
        }
        return total;
    }

    public void DecreaseItemQuantity(ItemData item, int useQuantity)    // 외부에서 인벤토리에 있는 아이템을 사용할 때 실행
    {
        for(int i = slots.Length - 1; i > 0; i--)
        {
            if (slots[i].Item == item)
            {
                if (slots[i].Quantity >= useQuantity)
                {
                    slots[i].Quantity -= useQuantity;
                    useQuantity = 0;
                    break;
                }
                else
                {
                    useQuantity -= slots[i].Quantity;
                    slots[i].Quantity = 0;
                    slots[i].Item = null;
                }
            }
        }

        // UI 업데이트
        UpdateUI();
    }
}
