using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPC : MonoBehaviour, IInteractable //상속?
{
    //[Header("NPC 세팅")]
     
    public void OnInteract()
    {
        Vector3 lookDir = GameManager.Instance.GetPlayer(0).transform.position - transform.position;
        lookDir.y = 0;
        Debug.DrawLine(transform.position, GameManager.Instance.GetPlayer(0).transform.position, Color.blue, 1.0f);
        gameObject.transform.DORotateQuaternion(Quaternion.LookRotation(lookDir.normalized), 0.5f).onComplete += UIManager.Instance.ActiveNPCDialouge;
        //UIManager.Instance.ActiveNPCDialouge(); // 대화 활성화
    }

    public void SetInteractionText()
    {
        UIManager.Instance.SetInteractionUIText("대화하기");
    }
}
