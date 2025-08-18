using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipItem", menuName = "New EquipItem")]

public class EquipItemData : ItemData
{
    [Header("장비 정보 세팅")]
    [SerializeField] GameObject equipPrefab; 
}
