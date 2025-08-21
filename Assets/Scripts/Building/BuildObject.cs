using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable
{
    public void SetRepairText();
    public void OnRepair();
}

public abstract class BuildObject : MonoBehaviour
{
    public Build build;

    public BuildData buildData;

    public int maxDurability;   // 총 내구도
    public int curDurability;   // 현재 내구도

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        buildData = build.buildData;
        build.activeBuild.Add(this);

        build.CancelPreview();

        curDurability = maxDurability;
    }

    public virtual void DestroyBuild()
    {
        build.activeBuild.Remove(this);
        Destroy(gameObject);
    }

    public virtual float RepairBuild()
    {
        float ratio = (float)curDurability / maxDurability;
        curDurability = maxDurability;
        return ratio;
    }
}
