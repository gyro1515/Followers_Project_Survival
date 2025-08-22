using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityObject : BuildObject, IInteractable, IRepairable
{
    public override void DestroyBuild()
    {
        build.activeBuild.Remove(this);
        Destroy(gameObject);
    }

    public override float RepairBuild()
    {
        float ratio = (float)curDurability / maxDurability;
        curDurability = maxDurability;
        return ratio;
    }

    public void SetInteractionText()
    {
        UIManager.Instance.SetInteractionUIText("제작하기");
    }

    public void OnInteract()
    {
        // CraftUI 켜주기
        UIManager.Instance.SetCraftUI();
    }

    public void SetRepairText()
    {
        UIManager.Instance.SetBuildTargetObject(this);
        UIManager.Instance.SetRepairUIText();
    }

    public void OnRepair()
    {
        // 수리 UI 켜주기
        UIManager.Instance.SetRepairWindow(this);
    }
}
