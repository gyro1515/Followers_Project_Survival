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
    public Vector3 addWorldPos; // 구조물 월드 좌표 조정용, 기준이 보통 바닥이기 때문에, 조정값 필요
    public Vector3 screenPos; // 해당 구조물 위치에서 화면 기준 어느 방향에 있을 것인가

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
