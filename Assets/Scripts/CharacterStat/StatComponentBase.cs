using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatComponentBase : MonoBehaviour
{
    public List<SO_StatDefinition> statDefinitions = new List<SO_StatDefinition>();
    public Dictionary<StatType, StatValue> statValues = new Dictionary<StatType, StatValue>();

    protected virtual void Awake()
    {
        Initialize();
    }
    protected virtual void Update()
    {

    }

    protected virtual void Start()
    {
        
    }
    protected virtual void Initialize()
    {
        foreach (SO_StatDefinition statDefinition in statDefinitions)
        {
            StatValue statValue = new StatValue();
            statValue.Initialize(statDefinition);
            statValues.Add(statDefinition.type, statValue);

        }
    }

    public StatValue GetStatValue(StatType type)
    {
        if (statValues.TryGetValue(type, out var statValue))
        {
            return statValue;

        }
        return null;
    }
}
