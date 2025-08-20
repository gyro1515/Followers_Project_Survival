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
    public IInteractable CurInteractable { get { return curInteractable; } }

    void Start()
    {
        _camera = Camera.main;
        selectionOutlineController = _camera.GetComponent<SelectionOutlineController>();
    }

    // Update is called once per frame
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer < checkRate) return;
        checkTimer -= checkRate; // 좀 더 정확한 시간 간격이 되게끔

        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            curInteractable = null;
            UIManager.Instance.DeactivateInteractionUI(); // UI 끄기
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
}
