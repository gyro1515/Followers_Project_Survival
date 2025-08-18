using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void OnInteract(); // 상호작용 가능한 것들은 OnInteract()에서 작성하도록
    // 예: ItemObject = 줍기, 문 = 열기, 작업대 = 작업하기 등

}
public class ItemObject : MonoBehaviour, IInteractable
{
    [Header("아이템 세팅")]
    [SerializeField] ItemData itemData;
    public void OnInteract()
    {
        // 인벤토리에 추가 후 삭제
        TestUISpawn.Instance.UIInventory.AddItem(itemData);
        Destroy(gameObject);
    }
}
