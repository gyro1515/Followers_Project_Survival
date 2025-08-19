using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("대화창 설정")]
    [SerializeField] RectTransform dialogueWindowRT;
    [SerializeField] TextMeshProUGUI npcDialogueText;
    [SerializeField] float endPosY;
    [SerializeField] float startPosY;
    [SerializeField] float duration = 0.3f; // 글자당 시간
    float totalDuration = 0f;
    string targetText;
    bool isFinish = false;
    private void Awake()
    {
        targetText = npcDialogueText.text;

    }
    private void OnEnable()
    {
        // 아마 여기서 글자 세팅하고
        // 아래 실행해야??

        // 랙트 트랜스폼 원상복구
        dialogueWindowRT.position = new Vector3(dialogueWindowRT.position.x, startPosY, dialogueWindowRT.position.z);
        dialogueWindowRT.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        dialogueWindowRT.DOAnchorPos3DY(endPosY, 0.5f).onComplete += ActiveText;

        totalDuration = targetText.Length * duration;
        npcDialogueText.text = "";
        //Debug.Log(npcDialogueText.text);
        
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
    public void SetText(string text) // 외부 호출 용
    {
        targetText = text;
        totalDuration = targetText.Length * duration;
    }
    
    void ActiveText()
    {
        Debug.Log("타이핑 시작");
        StartCoroutine("StartActiveText");
    }
    // 아래 두 함수는 테스트 용도
    void ActiveFinish()
    {
        Debug.Log("타이핑 끝");
        StartCoroutine("StartActiveFinish");
    }
    void SetActiveFalse()
    {
        Debug.Log("끔");
        StartCoroutine("StartSetActiveFalse");
    }
    
    IEnumerator StartActiveText()
    {
        yield return new WaitForSeconds(0.5f);
        int currentLength = 0;
        DOTween.To(
            () => currentLength,
            x =>
            {
                currentLength = x;
                npcDialogueText.text = targetText.Substring(0, currentLength);
                //Debug.Log(x);
                //if (currentLength == targetText.Length) ActiveFinsh();
            },
            targetText.Length,
            totalDuration
            ).SetEase(Ease.Linear).onComplete += ActiveFinish;
    }
    IEnumerator StartActiveFinish()
    {
        yield return new WaitForSeconds(0.5f);
        // 내려가기ver
        //dialogueWindowRT.DOAnchorPos3DY(startPosY, 0.5f).onKill += SetActiveFalse; 
        // 작아지기ver
        npcDialogueText.text = ""; // 글자 초기화
        dialogueWindowRT.DOScale(0f, 0.5f).onComplete += SetActiveFalse;
    }
    IEnumerator StartSetActiveFalse()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
