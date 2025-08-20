using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    public Build build;

    public BuildData buildData;

    public int maxDurability;   // 총 내구도
    public int curDurability;   // 현재 내구도

    private void Awake()
    {
        build = FindObjectOfType<Build>();

        buildData = build.buildData;
        build.activeBuild.Add(this);
    }

    void Start()
    {
        curDurability = maxDurability;
    }

    public void DestroyBuild()
    {
        build.activeBuild.Remove(this);
        Destroy(gameObject);
    }
}
