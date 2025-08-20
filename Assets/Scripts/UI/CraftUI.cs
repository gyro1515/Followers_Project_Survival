using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    public UIInventory inventory;

    [SerializeField] CraftData[] craftDatas;    // 건축물 데이터들
    [SerializeField] Button craftButton;
    [SerializeField] Button exitButton;

    [Header("List")]
    [SerializeField] GameObject listPrefab; // 건축물 리스트 프리팹
    [SerializeField] GameObject listContent;    // 리스트 스크롤 뷰 Content

    [Header("InfoUI")]
    [SerializeField] TextMeshProUGUI craftName;
    [SerializeField] TextMeshProUGUI craftDescription;
    [SerializeField] GameObject materialListPrefab; // 재료 리스트 프리팹
    [SerializeField] GameObject materialListContent;    // 재료 리스트 스크롤 뷰 Content

    [SerializeField] Image craftImage;   // 건축물 이미지
    [SerializeField] Sprite nullImage;   // 건축물 선택 안 했을 시 이미지

    CraftData selectedCraft;

    private void Awake()
    {
        craftDatas = Resources.LoadAll<CraftData>("CraftData");    // Resources 폴더 안에 CraftData 폴더 만들어서 CraftData있는 ScriptableObject 넣어주기
    }

    private void Start()
    {
        InitCraftList();
        //build = PlayerManager.Instance.player.build;
        gameObject.SetActive(false);

        // 버튼 이벤트 할당
        craftButton.onClick.AddListener(OnClickCraft);
        exitButton.onClick.AddListener(OnClickExit);
    }

    private void Update()
    {
        // 테스트용
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnClickCraft();
            }
        }
    }

    // 테스트를 위해 public으로 변경, 이후에 private로 변경해야함
    public void UpdateUI() // 처음에 UI 열때도 실행하기
    {
        if (selectedCraft == null)  // selectedCraft에 데이터가 없을 시 설정
        {
            // Craft 버튼 비활성화
            craftButton.interactable = false;

            // Material 리스트 초기화
            DestroyChildObject(materialListContent);

            // 아이템 이름, 설명, 이미지
            craftName.text = string.Empty;
            craftDescription.text = string.Empty;
            craftImage.sprite = nullImage;
        }
        else
        {
            craftButton.interactable = CheckHasAllMaterials();  // 재료가 충분하면 Craft 버튼 활성화

            // 아이템 이름, 설명, 이미지
            craftName.text = selectedCraft.data.DisplayName;
            craftDescription.text = selectedCraft.data.Description;
            craftImage.sprite = selectedCraft.data.Icon;

            // 재료 리스트 갱신
            UpdateCraftMaterial();
        }
    }

    public void OpenCraftUI()
    {
        selectedCraft = craftDatas[0];
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void CloseCraftUI()
    {
        selectedCraft = null;
        gameObject.SetActive(false);
    }

    public void OnClickCraftSlot(CraftData data)
    {
        selectedCraft = data;
        UpdateUI();
    }

    public void OnClickCraft()
    {
        // 아이템 생성
        CraftItem(selectedCraft);
    }

    public void OnClickExit()
    {
        CloseCraftUI();
    }

    void InitCraftList()
    {
        foreach (var craft in craftDatas)
        {
            GameObject go = Instantiate(listPrefab, listContent.transform);

            // 텍스트 세팅
            TextMeshProUGUI craftName = go.GetComponentInChildren<TextMeshProUGUI>();
            craftName.text = craft.data.DisplayName;

            // 버튼 onClick 이벤트 세팅
            Button button = go.GetComponentInChildren<Button>();
            var tempData = craft;
            button.onClick.AddListener(() => OnClickCraftSlot(tempData));
        }
    }

    void UpdateCraftMaterial()
    {
        // 이미 데이터가 있다면 다 삭제해야함
        DestroyChildObject(materialListContent);

        foreach (var material in selectedCraft.materials)
        {
            GameObject go = Instantiate(materialListPrefab, materialListContent.transform);

            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{material.materialData.DisplayName}\n{inventory.GetTotalQuantity(material.materialData)} / {material.requiredQuantity}";
            // 재료 이름
            // 현재 개수 / 필요 개수

            Image icon = go.GetComponentInChildren<Image>();
            icon.sprite = material.materialData.Icon;
        }
    }

    void DestroyChildObject(GameObject go)  // 오브젝트 풀링 이용하면 좋을려나
    {
        foreach (Transform child in go.transform)
        {
            Destroy(child.gameObject);
        }
    }

    bool CheckHasAllMaterials()
    {
        foreach (var material in selectedCraft.materials)
        {
            if (inventory.GetTotalQuantity(material.materialData) < material.requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }

    public void CraftItem(CraftData craftData)
    {
        inventory.AddItem(craftData.data);
        // 재료 감소
        foreach (var material in craftData.materials)
        {
            inventory.DecreaseItemQuantity(material.materialData, material.requiredQuantity);
        }
    }
}
