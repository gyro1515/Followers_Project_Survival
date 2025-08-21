using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    Collider collider;

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
        collider.isTrigger = !collider.isTrigger;
        isOpen = collider.isTrigger;
    }

    public void SetInteractionText()
    {
        if (!isOpen) UIManager.Instance.SetInteractionUIText("문 열기");
        else UIManager.Instance.SetInteractionUIText("문 닫기");
    }
}
