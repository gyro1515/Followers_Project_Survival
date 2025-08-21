using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    public Build build;

    public UIInventory inventory;

    [SerializeField] BuildData[] buildDatas;    // 건축물 데이터들
    [SerializeField] Button buildButton;
    [SerializeField] Button exitButton;

    [Header("List")]
    [SerializeField] GameObject listPrefab; // 건축물 리스트 프리팹
    [SerializeField] GameObject listContent;    // 리스트 스크롤 뷰 Content

    [Header("InfoUI")]
    [SerializeField] TextMeshProUGUI buildName;
    [SerializeField] TextMeshProUGUI buildDescription;
    [SerializeField] GameObject materialListPrefab; // 재료 리스트 프리팹
    [SerializeField] GameObject materialListContent;    // 재료 리스트 스크롤 뷰 Content

    [SerializeField] Image buildImage;   // 건축물 이미지
    [SerializeField] Sprite nullImage;   // 건축물 선택 안 했을 시 이미지

    [Header("오디오 클립")]
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip openCloseClip;

    BuildData selectedBuild;
    AudioSource audioSource;

    private void Awake()
    {
        buildDatas = Resources.LoadAll<BuildData>("BuildData");    // Resources 폴더 안에 BuildData 폴더 만들어서 BuildData있는 ScriptableObject 넣어주기

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        build = FindObjectOfType<Build>();

        InitBuildList();

        gameObject.SetActive(false);

        // 버튼 이벤트 할당
        buildButton.onClick.AddListener(OnClickBuild);
        exitButton.onClick.AddListener(OnClickExit);
    }

    private void Update()
    {
        // 테스트용
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnClickBuild();
            }
        }
    }

    // 테스트를 위해 public으로 변경, 이후에 private로 변경해야함
    public void UpdateUI() // 처음에 UI 열때도 실행하기
    {
        if (selectedBuild == null)  // selectedBuild에 데이터가 없을 시 설정
        {
            // Build 버튼 비활성화
            buildButton.interactable = false;

            // Material 리스트 초기화
            DestroyChildObject(materialListContent);

            // 건물 이름, 설명, 이미지
            buildName.text = string.Empty;
            buildDescription.text = string.Empty;
            buildImage.sprite = nullImage;
        }
        else
        {
            //buildButton.interactable = selectedBuild.CheckQuantity(currentQuantity);
            buildButton.interactable = CheckHasAllMaterials();  // 재료가 충분하면 Build 버튼 활성화

            // 건물 이름, 설명, 이미지
            buildName.text = selectedBuild.buildName;
            buildDescription.text = selectedBuild.description;
            buildImage.sprite = selectedBuild.buildImage;

            // 재료 리스트 갱신
            UpdateBuildMaterial();
        }
    }

    public void ToggleBuildUI()
    {
        if (gameObject.activeSelf)
        {
            CloseBuildUI();
        }
        else 
        {
            OpenBuildUI();
        }
        if(openCloseClip != null) audioSource.PlayOneShot(openCloseClip);
    }

    public void OpenBuildUI()
    {
        if (UIManager.Instance.IsAnyUIOn) return;
        UIManager.Instance.IsAnyUIOn = true;
        if (build.previewGameObject != null)
        {
            build.CancelPreview();
        }
        selectedBuild = buildDatas[0];
        gameObject.SetActive(true);
        UpdateUI();
        GameManager.Instance.SetCursorVisibility(true);
    }

    public void CloseBuildUI()
    {
        UIManager.Instance.IsAnyUIOn = false;
        selectedBuild = null;
        gameObject.SetActive(false);
        GameManager.Instance.SetCursorVisibility(false);
    }

    public void OnClickCancel()
    {
        build.CancelPreview();
        if (clickClip != null) audioSource.PlayOneShot(clickClip);
    }

    public void OnClickBuildSlot(BuildData data)
    {
        selectedBuild = data;
        UpdateUI();
        if (clickClip != null) audioSource.PlayOneShot(clickClip);
    }

    public void OnClickBuild()
    {
        // 미리보기 생성
        build.InitPreview(selectedBuild);
        // 소리 재생
        if (clickClip != null) audioSource.PlayOneShot(clickClip);
        // UI 끄기
        ToggleBuildUI();
    }

    public void OnClickExit()   // 얘는 클릭 소리 넣어야되나 닫는 소리 넣어야되나 흠..
    {
        CloseBuildUI();
    }

    void InitBuildList()
    {
        foreach(var build in buildDatas)
        {
            GameObject go = Instantiate(listPrefab, listContent.transform);

            // 텍스트 세팅
            TextMeshProUGUI buildName = go.GetComponentInChildren<TextMeshProUGUI>();
            buildName.text = build.buildName;

            // 버튼 onClick 이벤트 세팅
            Button button = go.GetComponentInChildren<Button>();
            var tempData = build;
            button.onClick.AddListener(() => OnClickBuildSlot(tempData));
        }
    }

    void UpdateBuildMaterial()
    {
        // 이미 데이터가 있다면 다 삭제해야함
        DestroyChildObject(materialListContent);

        foreach(var material in selectedBuild.materials)
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
        foreach(Transform child in go.transform)
        {
            Destroy(child.gameObject);
        }
    }

    bool CheckHasAllMaterials()
    {
        foreach (var material in selectedBuild.materials)
        {
            if(inventory.GetTotalQuantity(material.materialData) < material.requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }
}
