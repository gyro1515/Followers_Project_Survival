using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 UI에 네모 한칸을 개별 클래스로 작성
public class ItemSlot : MonoBehaviour
{
    UIInventory inventory;
    [SerializeField] ItemData item;   // 아이템 데이터, 외부에서 볼 수 있도록
    [SerializeField] Button button;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI quatityText;  // 수량표시 Text
    private Outline outline;  // 장비 장착시 Outline 표시위한 컴포넌트

    public ItemData Item {  get { return item; } set { item = value; } }
    int index;                    // 몇 번째 Slot인지 index 할당
    public bool equipped;         // 장착여부
    int quantity;                 // 수량데이터
    public int Quantity {  get { return quantity; }  set { quantity = value; } }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        button.onClick.AddListener(OnClickButton); // 코드로 연결하기, 강의는 인스펙터에서 연결
    }

    private void OnEnable() // SetActive(true)할때마다 호출
    {
        outline.enabled = equipped;
    }
    public void Init(UIInventory _inventory, int idx)
    {
        inventory = _inventory;
        index = idx;
    }
    // UI(슬롯 한 칸) 업데이트를 위한 함수
    // 아이템데이터에서 필요한 정보를 각 UI에 표시
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.Icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    // UI(슬롯 한 칸)에 정보가 없을 때 UI를 비워주는 함수
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    // 슬롯을 클릭했을 때 발생하는 함수.
    public void OnClickButton()
    {
        // 인벤토리의 SelectItem 호출, 현재 슬롯의 인덱스만 전달.
        inventory.SelectItem(index);
    }
}
