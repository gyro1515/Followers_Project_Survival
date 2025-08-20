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
    private StatType statType;
    public StatType StatType { get { return statType; } }

    private float minValue;
    public float MinValue { get { return minValue; } }

    private float maxValue;
    public float MaxValue { get { return maxValue; } }

    private float baseValue;
    public float BaseValue
    {
        get { return baseValue; }
        set
        {
            baseValue = value;
            Mathf.Clamp(baseValue, MinValue, MaxValue);
            RecalculateFinalValue();
        }
    }

    private float flatModifier;
    public float FlatModifier
    {
        get { return flatModifier; }
        set
        {
            flatModifier = value;
            RecalculateFinalValue();
        }
    }

    private float percentModifer;
    // 0.3 => 30%
    public float PercentModifier
    {
        get { return percentModifer; }
        set
        {
            percentModifer = value;
            RecalculateFinalValue();
        }
    }

    private float finalValue;
    public float FinalValue
    {
        get { return finalValue; }
        private set 
        {
            finalValue = value;

            OnValueChanged?.Invoke(new StatChangedEventArgs(statType, finalValue, minValue, maxValue));
        }
    }

    public event Action<StatChangedEventArgs> OnValueChanged;

    public void Initialize(SO_StatDefinition statDefinition)
    {
        statType = statDefinition.type;
        baseValue = statDefinition.baseValue;
        minValue = statDefinition.minValue;
        maxValue = statDefinition.maxValue;
        flatModifier = 0f;
        percentModifer = 0f;
        RecalculateFinalValue();
    }

    public void RecalculateFinalValue()
    {
        float result = (baseValue + flatModifier) * (1f + percentModifer);
        FinalValue = result;
    }
    
}
