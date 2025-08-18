using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실제 로직 구현
public class CharacterMovementComponent : MonoBehaviour
{
    public float walkSpeed;

    Rigidbody rb;
    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.freezeRotation = true;
    }

    public void Move(Vector2 inputVector)
    {
        if (inputVector.magnitude < 0.1f)
        {
            return;
        }
        Vector3 dir = transform.forward * inputVector.y + transform.right * inputVector.x; 
        Vector3 moveVector = dir * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);
    }
    

}
