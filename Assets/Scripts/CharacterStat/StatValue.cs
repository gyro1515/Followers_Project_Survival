using System;
using UnityEngine;

public class StatChangedEventArgs : EventArgs
{
    public StatType Type { get; }
    public float Current { get; }
    public float Min { get; }
    public float Max { get; }

    public StatChangedEventArgs(StatType type, float current, float min, float max)
    {
        Type = type;
        Current = current;
        Min = min;
        Max = max;
    }
}

// BaseValue: 레벨업, 스탯 상승 등을 반영하는 수치
// modifier: 버프, 디버프 효과 같이 잠시 적용되는 수치
// MinValue, MaxValue: BaseValue의 최소, 최대
// FinalValue: 계산식에 따라 최종 계산된 스탯값
// FinalValue = (BaseValue + FlatModifier) * (1 + PercentModifier)
[System.Serializable]
public class StatValue
{
    public SO_StatDefinition StatDefinition;
    public StatType statType;
    // public StatType StatType
    public float BaseValue;
    public float MinValue;
    public float MaxValue;
    public float FlatModifier;
    // 0.3 => 30%
    public float PercentModifier;
    public float FinalValue;

    public event Action<StatChangedEventArgs> OnValueChanged;

    public void Initialize()
    {
        statType = StatDefinition.type;
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

        OnValueChanged?.Invoke(new StatChangedEventArgs(statType, FinalValue, MinValue, MaxValue));
    }

    public void SetBaseValue(float value)
    {
        BaseValue = value;
        Mathf.Clamp(BaseValue, MinValue, MaxValue);

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
