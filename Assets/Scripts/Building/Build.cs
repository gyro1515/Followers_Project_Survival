using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Build : MonoBehaviour
{
    Camera camera;

    public BuildData[] buildDatas;  // 건축물 데이터들

    public UIInventory inventory;

    public PreviewBuild previewBuild;
    public BuildData buildData;
    public GameObject previewGameObject;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] float previewDistance; // 첫 프리뷰 생성 거리
    [SerializeField] float buildDistance;    // 건축 사정거리

    // 인스펙터에 나오는데 리스트를 없애니 오류가 떠서 NonSerialized
    [NonSerialized] public List<BuildObject> activeBuild = new List<BuildObject>(); // 현재 설치된 건축물 리스트
    List<BuildObject> removeBuild = new List<BuildObject>(); // 검사 후에 activeBuild에서 삭제할 오브젝트들 임시 저장
    bool isExistRemove = false; // activeBuild에서 없앨 오브젝트가 있는지 여부

    [SerializeField] float durabilityTickInterval;  // 내구도 감소 주기
    [SerializeField] float requiredRatio;   // 수리 시 요구 재료 비율

    Quaternion previewRotation; // 첫 프리뷰 생성시 회전값

    bool isBuildMode = false;

    private void Awake()
    {
        buildDatas = Resources.LoadAll<BuildData>("BuildData");    // Resources 폴더 안에 BuildData 폴더 만들어서 BuildData있는 ScriptableObject 넣어주기
    }

    private void Start()
    {
        camera = Camera.main;

        InvokeRepeating("ReduceDurability", 0, durabilityTickInterval);
    }

    private void Update()
    {
        if (isBuildMode)
        {
            UpdatePreviewPosition();
            if (Input.GetKeyDown(KeyCode.Mouse0) && previewBuild.CanBuild())
            {
                InitBuilding();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 재료 돌려주기
                ReturnMaterial();

                CancelPreview();
            }
        }
    }

    void ReduceDurability()
    {
        if (activeBuild.Count <= 0) return;
        foreach(var build in activeBuild)
        {
            build.curDurability--;
            if(build.curDurability <= 0)
            {
                isExistRemove = true;
                removeBuild.Add(build);
            }
        }

        if (isExistRemove)
        {
            foreach(var remove in removeBuild)
            {
                remove.DestroyBuild();
            }
            removeBuild.Clear();
        }

        isExistRemove = false;
    }

    void UpdatePreviewPosition()
    {
        // 화면 중앙에 레이 쏘기
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        Quaternion rotation;

        // 땅과 충돌이 됐을 때
        if (Physics.Raycast(ray, out hit, buildDistance, groundLayer))
        {
            // 위치 갱신
            previewGameObject.transform.position = hit.point;
            // 회전 갱신
            rotation = Quaternion.LookRotation(camera.transform.forward) * previewRotation;
            rotation.x = 0;
            rotation.z = 0;

            previewGameObject.transform.rotation = rotation;
        }
    }

    public void InitPreview(BuildData preview)
    {
        isBuildMode = true;
        
        buildData = preview;

        GameObject previewPrefab = preview.previewPrefab;

        // 재료 개수 감소
        foreach (var material in buildData.materials)
        {
            Debug.Log(material.materialData);
            Debug.Log(material.requiredQuantity);
            inventory.DecreaseItemQuantity(material.materialData, material.requiredQuantity);
        }

        // 땅 쪽으로 레이 쏘기
        RaycastHit hit;
        Vector3 initPosition;
        Quaternion rotation;

        // 땅과 충돌이 됐을 때
        if (Physics.Raycast(transform.position + transform.forward * previewDistance, Vector3.down, out hit, buildDistance, groundLayer))
        {
            // 위치 갱신
            initPosition = hit.point;
        }
        else
        {
            // 충돌 없으면 현재 위치에
            Physics.Raycast(transform.position, Vector3.down, out hit, buildDistance, groundLayer);
            initPosition = hit.point;
        }

        previewRotation = previewPrefab.transform.rotation;

        rotation = Quaternion.LookRotation(camera.transform.forward) * previewRotation;
        rotation.x = 0;
        rotation.z = 0;

        previewGameObject = Instantiate(previewPrefab, initPosition, rotation);
        previewGameObject.GetComponent<PreviewBuild>().build = this;
    }

    public void InitBuilding()
    {
        // buildPrefab 생성하고 그 안에있는 BuildObject의 build에 이 Build 넣어주기
        Instantiate(buildData.buildPrefab, previewGameObject.transform.position, previewGameObject.transform.rotation).GetComponent<BuildObject>().build = this;
    }

    public void CancelPreview()
    {
        isBuildMode = false;
        Destroy(previewGameObject);
        previewGameObject = null;
        buildData = null;
    }

    public void ReturnMaterial()
    {
        foreach(var material in buildData.materials)
        {
            inventory.AddItem(material.materialData, material.requiredQuantity);
        }
    }

    public void RepairBuild(BuildObject build)    // 사용하기 전에 재료 충분한지 확인하기
    {
        // 내구도 채우기
        build.curDurability = build.maxDurability;
        // 재료 계산해서 깎기
        float ratio = (float)build.curDurability / build.maxDurability;

        foreach(var material in build.buildData.materials)
        {
            inventory.DecreaseItemQuantity(material.materialData, Mathf.CeilToInt(material.requiredQuantity * ratio * requiredRatio));
        }
    }
}
