using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("HUD 세팅")]
    [SerializeField] Image hpBar;
    [SerializeField] Image hungerBar;
    [SerializeField] Image steminaBar;
    [SerializeField] Image thirstBar;

    public void SetHpBar(float value)
    {
        if (hpBar) hpBar.fillAmount = value;
    }
    public void SetHungerBar(float value)
    {
        if (hungerBar) hungerBar.fillAmount = value;
    }
    public void SetSteminaBar(float value)
    {
        if (steminaBar) steminaBar.fillAmount = value;
    }
    public void SetThirstBar(float value)
    {
        if (steminaBar) thirstBar.fillAmount = value;
    }
}
