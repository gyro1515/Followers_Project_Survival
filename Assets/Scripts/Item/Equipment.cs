using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [Header("무기 설정")]
    [SerializeField] EquipmentType equipmentType;
    [SerializeField] float damage; // 몬스터 공격에 사용될 데미지
    [SerializeField] int quantityPerHit; // 한 번에 최대 몇 개의 자원을 캘 수 있는가
    
    public EquipmentType EquipmentType { get { return equipmentType; } }
}
