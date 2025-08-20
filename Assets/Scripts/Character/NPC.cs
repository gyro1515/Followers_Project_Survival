using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable //상속?
{
    //[Header("NPC 세팅")]
     
public void OnInteract()
    {
        UIManager.Instance.ActiveNPCDialouge(); // 대화 활성화
    }

    public void SetInteractionText()
    {
        UIManager.Instance.SetInteractionUIText("대화하기");
    }
}
