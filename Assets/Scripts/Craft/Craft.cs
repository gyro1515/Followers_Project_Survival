using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{
    public UIInventory inventory;

    public CraftData craftData;

    private void Awake()
    {
        inventory = FindObjectOfType<UIInventory>();
    }

    public void CraftItem()
    {
        inventory.AddItem(craftData.data);
        // 재료 감소
        foreach(var material in craftData.materials)
        {
            inventory.DecreaseItemQuantity(material.materialData, material.requiredQuantity);
        }
    }
}
