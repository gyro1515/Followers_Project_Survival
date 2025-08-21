using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, IInteractable
{
    [Header("물 세팅")]
    [SerializeField] float addThirstValue = 10f;
    BoxCollider col;
    InteractionComponet interactionComponet;
    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        interactionComponet = GameManager.Instance.GetPlayer(0).GetComponent<InteractionComponet>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // 레이어 오버라이드 설정으로 플레이어만 호출됨
        interactionComponet.InWater(true, this);
        //Debug.Log("물 진입");
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<InteractionComponet>().CurInteractable = null;
        interactionComponet.InWater(false, this);
        //Debug.Log("물 탈출");
    }

    public void OnInteract()
    {
        // 물 마시기
        Debug.Log("물 마시기");
        GameManager.Instance.AddPlayerStatValue(StatType.Thirst, addThirstValue);
        // 물도 게임 매니저가 소환하고 게임 매니저에서 델리게이트 등록? 
    }

    public void SetInteractionText()
    {
        UIManager.Instance.SetInteractionUIText("물 마시기");
    }
}
