using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    public const string RESOURCES_BUILD_DATAS = "BuildDatas";   // BuildData ScriptableObject가 들어있는 폴더 이름

    // 테스트 용도로 public으로 설정, 끝나면 private
    public Build build;
    // 인벤토리 있어야 됨.
    // 인벤토리의 모든 슬롯을 순회하면서 해당 아이템이 총 몇 개 있는지 확인해야함
    // 그 과정을 BuildData에 있는 materials 수 만큼 반복해야함

    [SerializeField] BuildData[] buildDatas;    // 건축물 데이터들
    [SerializeField] Button buildButton;

    [Header("List")]
    [SerializeField] GameObject listPrefab; // 건축물 리스트 프리팹
    [SerializeField] GameObject listContent;    // 리스트 스크롤 뷰 Content

    [Header("InfoUI")]
    [SerializeField] TextMeshProUGUI buildName;
    [SerializeField] TextMeshProUGUI buildDescription;
    [SerializeField] GameObject materialListPrefab; // 재료 리스트 프리팹
    [SerializeField] GameObject materialListContent;    // 재료 리스트 스크롤 뷰 Content

    //[SerializeField] Image buildImage;   // 건축물 이미지
    //[SerializeField] Image nullImage;   // 건축물 선택 안 했을 시 이미지

    // 테스트 용도로 public으로 설정, 끝나면 private
    public BuildData selectedBuild;

    private void Awake()
    {
        //buildDatas = Resources.LoadAll<BuildData>(RESOURCES_BUILD_DATAS);    // Resources 폴더 만들고 그 안에 BuildDatas 폴더 만들어서 BuildData있는 ScriptableObject 넣어주기
        build = FindObjectOfType<Build>();
    }

    private void Start()
    {
        InitBuildList();
        //build = PlayerManager.Instance.player.build;
        gameObject.SetActive(false);
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
            //buildImage = nullImage;
        }
        else
        {
            //buildButton.interactable = selectedBuild.CheckQuantity(currentQuantity);
            buildButton.interactable = CheckHasAllMaterials();  // 재료가 충분하면 Build 버튼 활성화

            // 건물 이름, 설명, 이미지
            buildName.text = selectedBuild.buildName;
            buildDescription.text = selectedBuild.description;
            //buildImage = selectedBuild.buildImage;

            // 재료 리스트 갱신
            UpdateBuildMaterial();
        }
    }

    public void OpenBuildUI()
    {
        if(build.previewGameObject != null)
        {
            Destroy(build.previewGameObject);
        }
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void OnClickBuildSlot(BuildData data)
    {
        selectedBuild = data;
        UpdateUI();
    }

    public void OnClickBuild()
    {
        build.InitPreview(selectedBuild.previewPrefab);
        // 비활성화 투명으로 만들든 setactive를 이용하든 안 보이게 하기
        gameObject.SetActive(false);
        build.isBuildMode = true;
    }

    public void OnClickExit()
    {
        selectedBuild = null;
        gameObject.SetActive(false);
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
            text.text = $"{material.materialName}\n{CheckCurrentQuantity()} / {material.requiredQuantity}";
            // 재료 이름
            // 현재 개수 / 필요 개수

            Image icon = go.GetComponentInChildren<Image>();
            icon = material.materialIcon;
        }
    }

    void DestroyChildObject(GameObject go)  // 오브젝트 풀링 이용하면 좋을려나
    {
        foreach(Transform child in go.transform)
        {
            Destroy(child.gameObject);
        }
    }

    int CheckCurrentQuantity()
    {
        // 아이템 개수 갱신
        int currentQuantity = 0;
        // 인벤토리 슬롯 순회하면서 해당 아이템이 있으면 currentQuantity 늘려주기
        //for (int j = 0; j < ) 인벤토리 슬롯 순회하기
        //if (selectedBuild.materails[i].name == itemSlot.name) // 이름이 같다면~
        //currentQuantity += itemSlot.Quantity
        // 별개로 스크롤 뷰의 Content에 추가해주기
        return currentQuantity;
    }

    bool CheckHasAllMaterials()
    {
        foreach (var material in selectedBuild.materials)
        {
            if(CheckCurrentQuantity() < material.requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }
}
