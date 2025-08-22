using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBuild : MonoBehaviour
{
    public Build build;

    [SerializeField] private Material green;
    [SerializeField] private Material red;

    [SerializeField] private LayerMask groundLayer; // 지형 레이어

    private List<Collider> colliders = new List<Collider>();

    private void Start()
    {
        build.previewBuild = this;
    }

    private void Update()
    {
        ChangeColor();
    }

    void ChangeColor()  // 건축 가능하면 초록색, 아니면 빨간색 쓰읍 지금 업데이트에서 하면 무거울 거 같은데
    {
        if (CanBuild())
        {
            SetColor(green);
        }
        else
        {
            SetColor(red);
        }
    }

    void SetColor(UnityEngine.Material material)    // 매개변수로 주어진 material로 변경
    {
        foreach(Transform child in this.transform)
        {
            UnityEngine.Material[] materials = new UnityEngine.Material[child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            child.GetComponent<Renderer>().materials = materials;
        }
    }

    public bool CanBuild()  // Build 가능한지 bool값 반환
    {
        if(colliders.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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
