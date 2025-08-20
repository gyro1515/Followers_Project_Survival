using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    Camera camera;

    public UIInventory inventory;

    public BuildData buildData;
    public GameObject previewGameObject;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] float previewDistance; // 첫 프리뷰 생성 거리
    [SerializeField] float buildDistance;    // 건축 사정거리

    Quaternion previewRotation; // 첫 프리뷰 생성시 회전값

    public bool isBuildMode = false;

    private void Awake()
    {
        inventory = FindObjectOfType<UIInventory>();
    }

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        // 테스트용
        if (isBuildMode)
        {
            UpdatePreviewPosition();
            if (Input.GetKeyDown(KeyCode.Mouse0))
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
    }

    public void InitBuilding()
    {
        Instantiate(buildData.buildPrefab, previewGameObject.transform.position, previewGameObject.transform.rotation);
        CancelPreview();
    }

    public void CancelPreview()
    {
        isBuildMode = false;
        Destroy(previewGameObject);
        previewGameObject = null;
    }

    public void ReturnMaterial()
    {
        foreach(var material in buildData.materials)
        {
            inventory.AddItem(material.materialData, material.requiredQuantity);
        }
    }
}
