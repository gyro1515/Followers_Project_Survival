using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    private Camera camera;

    public GameObject previewGameObject;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] Vector3 previewPosition;   // 첫 프리뷰 생성 지점
    [SerializeField] float buildDistance;    // 건축 사정거리

    public bool isBuildMode = false;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if(isBuildMode) UpdatePreviewPosition();
    }

    void UpdatePreviewPosition()
    {
        // 화면 중앙에 레이 쏘기
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // 땅과 충돌이 됐을 때
        if (Physics.Raycast(ray, out hit, buildDistance, groundLayer))
        {
            // 위치 갱신
            previewGameObject.transform.position = hit.point;
        }
    }

    public void InitPreview(GameObject preview)
    {
        // 플레이어 만들면 위치 가져와서 previewPosition 설정해주기, 조금 앞에서 아래쪽으로 레이쏴서 그 포인트에서 설치되게 하면 될듯?
        //previewPosition = PlayerManager.Instance.player.transform.position을 이용해서 바로 앞에 있는 땅에다 설치
        previewGameObject = Instantiate(preview, previewPosition, Quaternion.identity);
    }
}
