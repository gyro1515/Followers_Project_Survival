using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingType
{
    Structure,  // 건축물
    Facility    // 제작대 같은 설치물
}

[Serializable]
public class BuildMaterial
{
    // 어떤 게 필요한지 어떻게 나누지? 나무, 돌 등을 나눠야함, 인벤토리에서 가져와야함
    public string materialName;
    // 재료 아이콘
    public Image materialIcon;
    // 얼마나 필요한지
    public int requiredQuantity;
}

// ScriptableObject를 만들 때 빠르기 만들기 위해 메뉴창에다가 추가
[CreateAssetMenu(fileName = "Build", menuName = "New Build")]
public class BuildData : ScriptableObject
{
    [Header("Info")]
    public string buildName;
    public string description;
    public BuildingType type;
    //public Sprite icon; // 아이콘도 필요한가 생각해 볼 필요 있음
    public GameObject buildPrefab;      // 실제 설치될 프리팹
    public GameObject previewPrefab;    // PreviewBuild를 달아놓은 프리팹
    public Image buildImage;    // 해당 건축물의 이미지

    [Header("Material")]
    public BuildMaterial[] materials;
    public bool hasAllMaterials;
}
