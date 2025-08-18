using System;
using UnityEngine;


[System.Serializable]
public class StatValue
{
    public SO_StatDefinition StatDefinition;
    public float BaseValue;
    public float MinValue;
    public float MaxValue;
    public float FlatModifier;
    // 0.3 => 30%
    public float PercentModifier;
    public float FinalValue;


    public event Action<float> OnValueChanged;

    public void Initialize()
    {
        BaseValue = StatDefinition.baseValue;
        MinValue = StatDefinition.minValue;
        MaxValue = StatDefinition.maxValue;
        FlatModifier = 0f;
        PercentModifier = 0f;
        RecalculateFinalValue();
    }

    public void RecalculateFinalValue()
    {
        SetFinalValue((BaseValue + FlatModifier) * (1f + PercentModifier));
    }

    public void SetFinalValue(float value)
    {
        FinalValue = value;
        Mathf.Clamp(FinalValue, MinValue, MaxValue);

        OnValueChanged?.Invoke(FinalValue);
    }

    public void SetBaseValue(float value)
    {
        BaseValue = value;
        RecalculateFinalValue();
    }

    public void SetFlatModifier(float value)
    {
        FlatModifier = value;
        RecalculateFinalValue();
    }

    public void SetPercentModifier(float value)
    {
        FlatModifier = value;
        RecalculateFinalValue();
    }
}
