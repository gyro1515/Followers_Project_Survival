using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Resource : MonoBehaviour
{
    [Header("자원 세팅")]
    [SerializeField] GameObject dropPrefab;
    [SerializeField] int capacity; //
    public event Action<Resource> OnResourceDepleted;
    //public void Gather(Vector3 hitPoint, Vector3 hitNormal, int quantityPerHit, ref int objCnt)
    public void Gather(Transform dropTransform, int quantityPerHit)
    {
        // 아이템 떨구는 개수는 1 ~ n으로 세팅하기
        StartCoroutine(DropItem(dropTransform, UnityEngine.Random.Range(1, quantityPerHit + 1)));
    }
    IEnumerator DropItem(Transform dropTransform, int quantityPerHit)
    {
        for (int i = 0; i < quantityPerHit; i++) // quantityPerHit에 따라 아이템 더 떨어지도록
        {
            capacity -= 1;
            // 맞은 위치에서 플레이어를 향하게 스폰하기
            // -> hit.point가 0 0 0일 때가 많음 = hit.distance = 0일때 문제 발생
            // 플레이어 양 옆으로 자원 떨어지게 하기
            Vector3 dropPoint = gameObject.transform.position + gameObject.transform.up * 2f + gameObject.transform.forward * 0.4f;
            dropPoint += UnityEngine.Random.Range(0, 2) == 0 ? gameObject.transform.right * 0.4f : -gameObject.transform.right * 0.4f;
            Instantiate(dropPrefab, dropPoint, UnityEngine.Random.rotation);

            if (capacity <= 0) break; // 다 캤다면 바로 파괴 되도록
            yield return new WaitForSeconds(0.1f);
        }

        if (capacity <= 0)
        {
            //objCnt--;
            OnResourceDepleted?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
