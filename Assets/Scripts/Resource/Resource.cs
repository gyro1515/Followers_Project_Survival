using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Resource : MonoBehaviour
{
    [Header("자원 세팅")]
    [SerializeField] GameObject dropPrefab;
    [SerializeField] int capacity; //

    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int quantityPerHit, ref int objCnt)
    {
        for (int i = 0; i < quantityPerHit; i++) // quantityPerHit에 따라 아이템 더 떨어지도록
        {
            if (capacity <= 0) break;

            capacity -= 1;
            // 맞은 위치 살짝 앞에서 떨어지도록
            hitNormal.y = 0;
            hitNormal = hitNormal.normalized;
            Instantiate(dropPrefab, hitPoint + hitNormal, Quaternion.LookRotation(hitNormal, Vector3.up)); // 추후 떨어지는 위치 수정해야 함
        }

        if (capacity <= 0)
        {
            objCnt--;
            Destroy(gameObject);
        }
    }
}
