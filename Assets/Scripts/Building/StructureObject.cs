using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureObject : BuildObject, IRepairable
{
    public void SetRepairText()
    {
        UIManager.Instance.SetRepairUIText();
    }

    public void OnRepair()
    {
        UIManager.Instance.SetRepairWindow(this);
    }
}
