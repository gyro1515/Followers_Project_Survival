using UnityEngine;

public class MonsterHitboxController : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] GameObject HeadHitbox;
    [SerializeField] GameObject RightArmHitbox;
    [SerializeField] GameObject LeftArmHitbox;
    bool isHitboxEnabled = false;

    public void EnableHitbox()
    {
        if (HeadHitbox != null && RightArmHitbox != null && LeftArmHitbox != null)
        {
            HeadHitbox.SetActive(true);
            RightArmHitbox.SetActive(true);
            LeftArmHitbox.SetActive(true);
        }
    }
    public void DisableHitbox()
    {
        if (HeadHitbox != null && RightArmHitbox != null && LeftArmHitbox != null)
        {
            HeadHitbox.SetActive(false);
            RightArmHitbox.SetActive(false);
            LeftArmHitbox.SetActive(false);
        }
    }
}