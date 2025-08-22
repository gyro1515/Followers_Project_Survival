using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class RepairUI : MonoBehaviour
{
    BuildData buildData; // 수리할 건축물 데이터
    BuildObject buildObject;

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

    [Header("내구도 UI")]
    [SerializeField] GameObject durabilityPanel; // 움직일 패널
    [SerializeField] TextMeshProUGUI buildNameText;
    [SerializeField] Image durabilityBar;
    
    RectTransform durabilityPanelRectTransform;
    Camera cam;
    BuildObject targetBuildObject; // 위랑 중복되지만 혹시 몰라서 또 만들었습니다.
    private void Awake()
    {
        interactionBG.sizeDelta = new Vector2(interactionText.preferredWidth, interactionBG.sizeDelta.y);

        audioSource = gameObject.GetComponent<AudioSource>();

        repairButton.onClick.AddListener(OnClickRepair);
        exitButton.onClick.AddListener(OnClickExit);
        cam = Camera.main;
        durabilityPanelRectTransform = durabilityPanel.GetComponent<RectTransform>();
    }

    private void Start()
    {
        repairWindow.SetActive(false);
        interaction.SetActive(false);
        durabilityPanel.SetActive(false);

    }
    private void Update()
    {
        SetUIPos();
    }
    public void SetText()
    {
        if (interaction.gameObject.activeSelf) return;
        interaction.SetActive(true);
        buildNameText.text = targetBuildObject.buildData.buildName;
        durabilityBar.fillAmount = targetBuildObject.curDurability / (float)targetBuildObject.maxDurability;
        durabilityPanel.SetActive(true);
    }

    public void DeactiveText()
    {
        interaction.SetActive(false);
        durabilityPanel.SetActive(false);
        targetBuildObject = null;
    }

    public void OpenRepairWindow(BuildObject buildObject)
    {
        // UI매니저 UI 열려있는 상태로 설정
        if (UIManager.Instance.IsAnyUIOn) return;
        UIManager.Instance.IsAnyUIOn = true;

        // UI 활성화
        repairWindow.SetActive(true);

        // 필요한 데이터 할당
        this.buildObject = buildObject;
        buildData = buildObject.buildData;

        // 리스트 스크롤 뷰 콘텐츠 생성 후 갱신
        InitMaterialList();
        UpdateUI();

        // 소리 재생
        if (openCloseClip != null) audioSource.PlayOneShot(openCloseClip);

        // 마우스 커서 활성화
        GameManager.Instance.SetCursorVisibility(true);
    }

    void CloseRepairWindow()
    {
        // UI 닫힘 상태로 설정
        UIManager.Instance.IsAnyUIOn = false;

        // UI 비활성화
        repairWindow.SetActive(false);

        // 필요없는 데이터 삭제
        buildData = null;
        buildObject = null;

        // 리스트 스크롤 뷰 콘텐츠 하위 오브젝트 삭제
        DestroyChildObject(listContent);

        // 마우스 커서 비활성화
        GameManager.Instance.SetCursorVisibility(false);
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
    void SetUIPos()
    {
        if (!durabilityPanel.activeSelf) return;
        if (targetBuildObject == null) return;
        Vector3 tmpUIPos = cam.WorldToScreenPoint(targetBuildObject.gameObject.transform.position + targetBuildObject.addWorldPos);
        durabilityPanelRectTransform.position = tmpUIPos + targetBuildObject.screenPos;
        // 위치도 갱신하고 게이지도 갱신하기
        durabilityBar.fillAmount = targetBuildObject.curDurability / (float)targetBuildObject.maxDurability;

    }
    public void SetObjectTarget(BuildObject buildObject)
    {
        targetBuildObject = buildObject;
    }
}
