using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] StructureObject structure;
    // 테스트용 public, 이후 private로 전환
    public Collider collider;

    bool isOpen;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void OnInteract()
    {
        ToggleDoor();
    }

    void ToggleDoor()
    {
        // 문 열기
        // 파괴해야되나? ㄴㄴ 비활성화
        collider.isTrigger = !collider.isTrigger;
        isOpen = collider.isTrigger;
    }

    public void SetInteractionText()
    {
        if (!isOpen) UIManager.Instance.SetInteractionUIText("문 열기");
        else UIManager.Instance.SetInteractionUIText("문 닫기");
    }
}
