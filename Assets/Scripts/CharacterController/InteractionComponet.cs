using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponet : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] float checkRate = 0.05f;    // 상호작용 오브젝트 체크 시간
    [SerializeField] float maxCheckDistance;     // 최대 체크 거리
    [SerializeField] LayerMask layerMask;        // 충돌 체크할 레이어

    private float checkTimer;          // 체크 시간용 타이머
    private Camera _camera;
    private SelectionOutlineController selectionOutlineController;

    private IInteractable curInteractable;
    private IInteractable waterInteratable;
    public IInteractable CurInteractable { get { return curInteractable; } set { curInteractable = value; } }

    bool bIsInWater = false;
    void Awake()
    {
        _camera = Camera.main;
        selectionOutlineController = _camera.GetComponent<SelectionOutlineController>();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer < checkRate) return;
        checkTimer -= checkRate; // 좀 더 정확한 시간 간격이 되게끔

        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            if(!bIsInWater) // 플레이어가 물에 없을때만
            {
                curInteractable = null;
                UIManager.Instance.DeactivateInteractionUI(); // UI 끄기
            }
            else // 물에 있을 때는 다시 물 상호작용으로 바꾸기
            {
                curInteractable = waterInteratable;
                curInteractable?.SetInteractionText();
            }
            selectionOutlineController?.RemoveOutline();
            return; // 상호작용만 체크
        }
        if (!hit.collider.TryGetComponent(out IInteractable interactableForText)) return; // 이중 체크 -> 굳이?
        curInteractable = interactableForText;
        interactableForText.SetInteractionText();
        selectionOutlineController?.ApplyOutline(hit);
    }
    public void OnIteract()
    {
        curInteractable?.OnInteract();
    }
    public void InWater(bool _isInWater, IInteractable _interact)
    {
        bIsInWater = _isInWater;
        if (bIsInWater)
        {
            //enabled = false; // 기획에 따라 레이캐스트 감지를 정지할 수도 있음
            curInteractable = _interact;
            waterInteratable = _interact;
            curInteractable?.SetInteractionText();
        }
        else
        {
            //enabled = true;
            curInteractable = null;
            waterInteratable = null;
            UIManager.Instance.DeactivateInteractionUI();
        }
    }
}
