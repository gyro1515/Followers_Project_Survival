using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    [SerializeField] Camera camera;

    public BuildData buildData;
    public GameObject previewGameObject;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] Vector3 previewPosition;   // 첫 프리뷰 생성 지점
    Quaternion previewRotation; // 첫 프리뷰 생성시 회전값
    [SerializeField] float buildDistance;    // 건축 사정거리

    public bool isBuildMode = false;

    private void Update()
    {
        if (isBuildMode)
        {
            UpdatePreviewPosition();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                InitBuilding();
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

    public void InitPreview(GameObject preview)
    {
        // 플레이어 만들면 위치 가져와서 previewPosition 설정해주기, 조금 앞에서 아래쪽으로 레이쏴서 그 포인트에서 설치되게 하면 될듯?
        //previewPosition = PlayerManager.Instance.player.transform.position을 이용해서 바로 앞에 있는 땅에다 설치

        // 땅 쪽으로 레이 쏘기
        RaycastHit hit;
        Vector3 initPosition;

        // 땅과 충돌이 됐을 때
        if (Physics.Raycast(previewPosition + transform.position, Vector3.down, out hit, buildDistance, groundLayer))
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

        previewRotation = preview.transform.rotation;
        previewGameObject = Instantiate(preview, initPosition, previewRotation);
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
    }
}
