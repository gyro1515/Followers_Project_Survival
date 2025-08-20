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
    protected virtual void Initialize()
    {
        foreach (SO_StatDefinition statDefinition in statDefinitions)
        {
            StatValue statValue = new StatValue();
            statValue.StatDefinition = statDefinition;
            statValue.Initialize();
            statValues.Add(statDefinition.type, statValue);

        }
    }
}
