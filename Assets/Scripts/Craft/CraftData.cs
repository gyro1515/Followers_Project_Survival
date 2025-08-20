using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftMaterial
{
    // 어떤 게 필요한지
    public ItemData materialData;
    // 얼마나 필요한지
    public int requiredQuantity;
}

// ScriptableObject를 만들 때 빠르기 만들기 위해 메뉴창에다가 추가
[CreateAssetMenu(fileName = "Craft", menuName = "New Craft")]
public class CraftData : ScriptableObject
{
    [Header("Info")]
    public ItemData data;   // 만들 아이템 데이터

    [Header("Material")]
    public CraftMaterial[] materials;
    public bool hasAllMaterials;
}
