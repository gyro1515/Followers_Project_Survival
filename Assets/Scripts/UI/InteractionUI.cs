using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] RectTransform bG;

    private void Awake()
    {
        bG.sizeDelta = new Vector2(interactionText.preferredWidth, bG.sizeDelta.y);
        //gameObject.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetText("잘 확장이 되나?");
        }
    }
    public void SetText(string value)
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        interactionText.text = $"[E] {value}";
        bG.sizeDelta = new Vector2(interactionText.preferredWidth, bG.sizeDelta.y);
    }
}
