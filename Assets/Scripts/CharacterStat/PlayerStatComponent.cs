using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class StatRegenConfig
{
    public float Delay = 0f;
    public float Rate = 0.1f;
    public float Amount;

    public StatRegenConfig(float delay, float rate, float amount)
    {
        Delay = delay;
        Rate = rate;
        Amount = amount;
    }
}

[System.Serializable]
public class StatCostConfig
{
    public float Delay = 0f;
    public float Rate = 0.1f;
    public float Amount;

    public StatCostConfig(float delay, float rate, float amount)
    {
        Delay = delay;
        Rate = rate;
        Amount = amount;
    }
}

public class PlayerStatComponent : StatComponentBase
{
    private Dictionary<StatType, StatRegenConfig> regenConfigs = new();
    private Dictionary<StatType, StatCostConfig> costConfigs = new();

    private Dictionary<StatType, Coroutine> regenDelayCoroutines = new Dictionary<StatType, Coroutine>();

    private Dictionary<StatType, Coroutine> regenCoroutines = new Dictionary<StatType, Coroutine>();
    private Dictionary<StatType, Coroutine> costCoroutines = new Dictionary<StatType, Coroutine>();

    [SerializeField]
    private float sprintCost = 1f;
    [SerializeField]
    private float sprintCostRate = 0.1f;
    Coroutine sprintCoroutine;
    

    protected override void Awake()
    {
        base.Awake();

        regenConfigs.Add(StatType.Stamina, new StatRegenConfig(1f, 0.1f, 2f));
        costConfigs.Add(StatType.Stamina, new StatCostConfig(0f, 0.1f, 1f));
        costConfigs.Add(StatType.Hunger, new StatCostConfig(0f, 0.1f, 3f));
        costConfigs.Add(StatType.Thirst, new StatCostConfig(0f, 0.1f, 2f));
        costConfigs.Add(StatType.Health, new StatCostConfig(0f, 0.1f, 1f));


    }
    protected override void Initialize()
    {
        base.Initialize();

        
    }

    protected override void Start()
    {
        base.Start();

        DrainStat(StatType.Thirst);
        GetStatValue(StatType.Thirst).OnValueChanged += CheckStatValueZero;
        DrainStat(StatType.Hunger);
        GetStatValue(StatType.Hunger).OnValueChanged += CheckStatValueZero;

    }

    private void CheckStatValueZero(StatChangedEventArgs obj)
    {
        if (obj.Current == 0f)
        {
            Debug.Log($"{obj.Type} is zero");
            DrainStat(StatType.Health);
        }
    }

    private void DrainStat(StatType statType)
    {
        StatCostConfig config = costConfigs[statType];
        if (regenCoroutines.TryGetValue(statType, out var regenCoroutine))
        {
            StopCoroutine(regenCoroutine);
        }
        costCoroutines.TryAdd(statType, StartCoroutine(DrainStat(statType, config.Rate, config.Amount)));

    }

    protected override void Update()
    {
        base.Update();

    }

    // 스태미나 사용은 이 함수를 통해서 하기
    public void UseStamina(float amount)
    {
        StatValue stamina = GetStatValue(StatType.Stamina);
        if (stamina.BaseValue < amount)
        {
            return;
        }

        stamina.BaseValue -= amount;

        if (regenDelayCoroutines.ContainsKey(StatType.Stamina))
        {
            StopCoroutine(regenDelayCoroutines[StatType.Stamina]);
        }

        regenDelayCoroutines[StatType.Stamina] = StartCoroutine(DelayedRegenStat(StatType.Stamina, regenConfigs[StatType.Stamina]));

    }

    public void OnJump()
    {
        UseStamina(5f);
    }

    public void OnSprintEnter()
    {
        StatValue stamina = GetStatValue(StatType.Stamina);

        if (stamina.BaseValue < sprintCost) return;

        regenCoroutines.TryGetValue(StatType.Stamina, out var coroutine);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            regenCoroutines.Remove(StatType.Stamina);
        }
        sprintCoroutine = StartCoroutine(SprintCoroutine());
    }

    IEnumerator SprintCoroutine()
    {
        GetStatValue(StatType.MoveSpeed).BaseValue = 10f;

        StatValue statValue = GetStatValue(StatType.Stamina);
        while (statValue.BaseValue > statValue.MinValue)
        {
            yield return new WaitForSeconds(sprintCostRate);

            UseStamina(sprintCost);

        }

        GetStatValue(StatType.MoveSpeed).BaseValue = 5f;

    }

    public void OnSprintExit()
    {
        if (sprintCoroutine != null)
        {
            StopCoroutine(sprintCoroutine);
            GetStatValue(StatType.MoveSpeed).BaseValue = 5f;

        }
    }


    IEnumerator DrainStat(StatType statType, float rate, float costAmount)
    {
        StatValue statValue = GetStatValue(statType);

        while (statValue.BaseValue > statValue.MinValue)
        {
            yield return new WaitForSeconds(rate);

            statValue.BaseValue -= costAmount;

        }
    }

    IEnumerator DelayedRegenStat(StatType statType, StatRegenConfig regenConfig)
    {
        yield return new WaitForSeconds(regenConfig.Delay);

        regenCoroutines.TryAdd(statType, StartCoroutine(RegenStat(statType, regenConfig.Rate, regenConfig.Amount)));
        regenDelayCoroutines.Remove(statType);
    }

    

    IEnumerator RegenStat(StatType statType, float rate, float amount)
    {
        StatValue statValue = GetStatValue(statType);

        while (statValue.BaseValue < statValue.MaxValue)
        {
            yield return new WaitForSeconds(rate);
            statValue.BaseValue += amount;
        }
    }

    


}
