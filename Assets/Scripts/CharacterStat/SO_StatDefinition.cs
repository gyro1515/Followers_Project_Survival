using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    Health,
    Attack,
    Defense,
    MoveSpeed,
    Hunger,
    Stamina,
    Thirst,

}

[CreateAssetMenu(fileName = "NewStatDefinition", menuName = "Stats/Stat Definition")]
public class SO_StatDefinition : ScriptableObject
{
    public StatType type;
    public float baseValue;
    public float minValue;
    public float maxValue;
}
