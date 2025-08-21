using UnityEngine;

public class MonsterHitboxController : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] GameObject HeadHitbox;


    public void EnableHitbox()
    {
        if (HeadHitbox != null)
        {
            GetComponent<MonsterCharacter>().isAttacked = false;
            HeadHitbox.SetActive(true);
        }
    }
    public void DisableHitbox()
    {
        if (HeadHitbox != null)
        {
            HeadHitbox.SetActive(false);
        }
    }
}