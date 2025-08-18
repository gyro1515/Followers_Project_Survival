using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBuild : MonoBehaviour
{
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    [SerializeField] private LayerMask groundLayer; // 지형 레이어

    private List<Collider> colliders;

    private Material[] materials;

    private void Awake()
    {
        materials = GetComponentInChildren<Renderer>().materials;
    }

    private void Update()
    {
        ChangeColor();
    }

    void ChangeColor()  // 건축 가능하면 초록색, 아니면 빨간색
    {
        if (canBuild())
        {
            SetColor(green);
        }
        else
        {
            SetColor(red);
        }
    }

    void SetColor(Material material)    // 매개변수로 주어진 material로 변경
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i] = material;
        }
    }

    public bool canBuild()  // Build 가능한지 bool값 반환
    {
        return colliders.Count == 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // groundLayer에 포함되어 있지 않다면
        if ((groundLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // groundLayer에 포함되어 있지 않다면
        if ((groundLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            colliders.Remove(other);
        }
    }
}
