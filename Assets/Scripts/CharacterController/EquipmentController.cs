using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    None,
    Sword,
    Pickaxe,
    // 아이템 개수가 많아질수록 추가해야함... Sword1 등으로 확장 필요
}
public class EquipmentController : MonoBehaviour
{
    [Header("무기 장착 세팅")]
    [SerializeField] Transform weaponSocket;
    Equipment curEquipment;
    Dictionary<EquipmentType, Equipment> equipments = new Dictionary<EquipmentType, Equipment>(); // 장비 재사용하기

    public void EquipWeapon(GameObject equipmentGO)
    {
        Debug.Log("장비 추가");

        Equipment checkEquipment = equipmentGO.GetComponent<Equipment>();
        if (checkEquipment == null) return; // 무기가 아니라면 장착x
        if (!equipments.ContainsKey(checkEquipment.EquipmentType)) 
        {
            // 해당 장비 없다면 추가
            equipments.Add(checkEquipment.EquipmentType, Instantiate(equipmentGO, weaponSocket).GetComponent<Equipment>());
        }
        // 이미 해당 장비 있다면 다른 장비 해제하고 이 장비 장착
        SetCurEquipWeapon(checkEquipment.EquipmentType);
    }
    void SetCurEquipWeapon(EquipmentType equipmentType)
    {
        foreach (var equipment in equipments)
        {
            if(equipmentType == equipment.Value.EquipmentType)
            {
                equipment.Value.gameObject.SetActive(true);
                curEquipment = equipment.Value;
                Debug.Log("장착 완료");
            }
            else
            {
                equipment.Value.gameObject.SetActive(false);
            }
        }
    }
    public void UnEquipWeapon()
    {
        curEquipment?.gameObject.SetActive(false);
        curEquipment = null;
    }
}
