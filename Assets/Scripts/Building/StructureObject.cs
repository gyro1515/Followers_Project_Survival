using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureObject : BuildObject, IRepairable
{
    void Start()
    {
        buildData = build.buildData;
        Debug.Log(buildData);
        build.activeBuild.Add(this);
        Debug.Log(build.activeBuild[0]);

        build.CancelPreview();

        curDurability = maxDurability;
    }


    public void SetRepairText()
    {
        UIManager.Instance.SetRepairUIText();
    }

    public void OnRepair()
    {
        UIManager.Instance.SetRepairWindow(this);
    }
}
