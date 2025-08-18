using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MiningItem", menuName = "New MiningItem")]
public class MiningItemData : ItemData
{
    [Header("마이닝 아이템 정보 세팅")]
    [SerializeField] int maxStackAmount; // 한 칸에 최대 몇개까지 보유할 수 있는가
    public int MaxStackAmount { get { return maxStackAmount; } }

}
