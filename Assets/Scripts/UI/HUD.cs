using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Dictionary<StatType, Image> statBars = new Dictionary<StatType, Image>();
    [Header("HUD 세팅")]
    [SerializeField] Image hpBar;
    [SerializeField] Image hungerBar;
    [SerializeField] Image staminaBar;
    [SerializeField] Image thirstBar;

    private void Awake()
    {
        statBars.Add(StatType.Health, hpBar);
        statBars.Add(StatType.Hunger, hungerBar);
        statBars.Add(StatType.Stamina, staminaBar);
        statBars.Add(StatType.Thirst, thirstBar);
    }

    public void UpdateGaugeBar(StatChangedEventArgs eventArgs)
    {
        float percent = eventArgs.Current / eventArgs.Max;

        Image bar;
        if (statBars.TryGetValue(eventArgs.Type, out bar))
        {
            bar.fillAmount = percent;
        }
        
    }

    
}
