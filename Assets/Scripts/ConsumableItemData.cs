using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ConsumableType
{
    Hunger,
    Health
}
[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}
[CreateAssetMenu(fileName = "ConsumableItem", menuName = "New ConsumableItem")]
public class ConsumableItemData : ItemData
{
    [Header("소비 아이템 정보")]
    [SerializeField] ItemDataConsumable[] consumables; 
    public ItemDataConsumable[] Consumables { get { return consumables; } }
}
