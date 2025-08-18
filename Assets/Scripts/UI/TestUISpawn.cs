using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUISpawn : SingletonMono<TestUISpawn>
{
    // 싱글톤으로 불러가기

    [SerializeField] GameObject inventoryPrefab;
    private UIInventory uiInventory;
    public UIInventory UIInventory { get { return uiInventory; } }
    protected override void Awake()
    {
        base.Awake();
        uiInventory = Instantiate(inventoryPrefab, gameObject.transform).GetComponent<UIInventory>();
    }
    private void Update()
    {
        // 테스트
        if(uiInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            uiInventory.Toggle();
        }
    }
}
