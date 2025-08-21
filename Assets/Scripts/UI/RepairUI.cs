using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairUI : MonoBehaviour
{
    public BuildData buildData; // 수리할 건축물 데이터, 이거 어케 가져옴??
    public BuildObject buildObject;

    public Build build;
    public UIInventory inventory;

    AudioSource audioSource;

    [SerializeField] float requiredRatio;   // 수리 시 필요한 재료 비율(전체 재료 개수에 비례)

    [Header("상호작용 설정")]
    [SerializeField] GameObject interaction;
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] RectTransform interactionBG;

    [Header("수리")]
    [SerializeField] GameObject repairWindow;
    [SerializeField] Button repairButton;
    [SerializeField] Button exitButton;

    [Header("재료 리스트")]
    [SerializeField] GameObject listPrefab; // 수리 재료 리스트 프리팹
    [SerializeField] GameObject listContent;    // 수리 재료 리스트 스크롤 뷰 Content

    [Header("오디오 클립")]
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip openCloseClip;

    private void Awake()
    {
        interactionBG.sizeDelta = new Vector2(interactionText.preferredWidth, interactionBG.sizeDelta.y);
        //gameObject.SetActive(false);

        audioSource = gameObject.GetComponent<AudioSource>();

        repairButton.onClick.AddListener(OnClickRepair);
        exitButton.onClick.AddListener(OnClickExit);
    }

    private void Start()
    {
        repairWindow.SetActive(false);
        interaction.SetActive(false);
    }

    public void SetText()
    {
        interaction.SetActive(true);
        interactionText.text = $"[F] 수리하기";
        interactionBG.sizeDelta = new Vector2(interactionText.preferredWidth, interactionBG.sizeDelta.y);
    }

    public void DeactiveText()
    {
        interaction.SetActive(false);
    }

    public void OpenRepairWindow(BuildObject buildObject)
    {
        this.buildObject = buildObject;
        buildData = buildObject.buildData;
        InitMaterialList();
        UpdateUI();
        if (openCloseClip != null) audioSource.PlayOneShot(openCloseClip);
    }

    void CloseRepairWindow()
    {
        buildData = null;
        buildObject = null;
        DestroyChildObject(listContent);
    }

    void UpdateUI()
    {
        // repair 버튼 활성화
        repairButton.interactable = CheckHasAllMaterials();
    }

    void Repair()
    {
        float ratio = buildObject.RepairBuild();

        // 재료 깎기
        foreach (var material in buildData.materials)
        {
            inventory.DecreaseItemQuantity(material.materialData, Mathf.CeilToInt(material.requiredQuantity * ratio * requiredRatio));
        }
    }

    public void OnClickRepair()
    {
        Repair();
        CloseRepairWindow();
        if (clickClip != null) audioSource.PlayOneShot(clickClip);
    }

    void Exit()
    {
        CloseRepairWindow();
    }

    public void OnClickExit()
    {
        Exit();
        if (openCloseClip != null) audioSource.PlayOneShot(openCloseClip);
    }
    
    void InitMaterialList()
    {
        foreach(var material in buildData.materials)
        {
            GameObject go = Instantiate(listPrefab, listContent.transform);

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
        foreach (var material in buildData.materials)
        {
            if (inventory.GetTotalQuantity(material.materialData) < material.requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }
}
