using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private IRepairable curRepairable;
    public IRepairable CurRepairalbe { get { return curRepairable; } set { curRepairable = value; } }

    bool bIsInWater = false;
    bool bIsInteractable = false;
    bool bIsRepairable = false;
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
            curRepairable = null;
            UIManager.Instance.DeactivateRepairInteractionUI();

            selectionOutlineController?.RemoveOutline();
            return; // 상호작용만 체크
        }
        bIsInteractable = hit.collider.TryGetComponent(out IInteractable interactableForText);
        bIsRepairable = hit.collider.TryGetComponent(out IRepairable repairable);
        //bIsRepairable = hit.collider.CompareTag("Build");
        Debug.Log(bIsRepairable);
        if (!bIsInteractable && !bIsRepairable) return; // 이중 체크 -> 굳이?
        if (bIsInteractable)
        {
            curInteractable = interactableForText;
            interactableForText.SetInteractionText();
            selectionOutlineController?.ApplyOutline(hit);
        }
        else
        {
            curInteractable = null;
            UIManager.Instance.DeactivateInteractionUI(); // UI 끄기
        }
        if (bIsRepairable)
        {
            curRepairable = repairable;
            // ui 켜주기
            UIManager.Instance.SetRepairUIText();
        }
        else
        {
            curRepairable = null;
            UIManager.Instance.DeactivateRepairInteractionUI();
        }
        //curInteractable = interactableForText;
        //interactableForText.SetInteractionText();
        //selectionOutlineController?.ApplyOutline(hit);
    }
    public void OnIteract()
    {
        curInteractable?.OnInteract();
    }
    public void OnRepair()
    {
        Debug.Log("인터랙션 컴포넌트에서 실행");
        curRepairable?.OnRepair();
    }
    public void InWater(bool _isInWater, IInteractable _interact)
    {
        bIsInWater = _isInWater;
        if (bIsInWater)
        {
            //enabled = false; // 기획에 따라 레이캐스트 감지를 정지할 수도 있음
            waterInteratable = _interact;
            // 아이템을 가리키면서 물에 들어가는 경우가 아니라면
            if (curInteractable == null) 
            {
                curInteractable = _interact;
                curInteractable?.SetInteractionText();
            }
        }
        else
        {
            //enabled = true;
            // 물에서 나올때 아이템을 가리키지 않는 상황이라면
            if (curInteractable == waterInteratable)
            {
                UIManager.Instance.DeactivateInteractionUI();
            }
            waterInteratable = null;
        }
    }
}
