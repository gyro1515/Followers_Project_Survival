using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject를 만들 때 빠르기 만들기 위해 메뉴창에다가 추가
[CreateAssetMenu(fileName = "Craft", menuName = "New Craft")]
public class CraftData : ScriptableObject
{
    [Header("Info")]
    public ItemData data;   // 만들 아이템 데이터

    [Header("Material")]
    public ItemMaterial[] materials;
    public bool hasAllMaterials;
}
