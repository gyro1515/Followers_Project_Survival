using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour, IInteractable
{
    public Build build;

    public BuildData buildData;

    public int maxDurability;   // 총 내구도
    public int curDurability;   // 현재 내구도

    private void Awake()
    {
        //build = FindObjectOfType<Build>();

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

    public void SetInteractionText()
    {
        UIManager.Instance.SetInteractionUIText("제작하기");
    }

    public void OnInteract()
    {
        // CraftUI 켜주기
        UIManager.Instance.SetCraftUI();
    }
}
