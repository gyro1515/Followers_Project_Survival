using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실제 로직 구현
public class CharacterMovementComponent : MonoBehaviour
{
    public float walkSpeed;
    public float jumpForce = 5f;

    Rigidbody rb;

    bool isJumping = false;
    public bool IsJumping {  get { return isJumping; } }
    bool isFalling = false;
    public bool IsFalling { get {return isFalling; } }
    bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isFalling = !isGrounded && rb.velocity.y < 0f;
        DetectGround();
    }

    void DetectGround()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = Vector3.down;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 1.1f))
        {
            isGrounded = true;
            if (isJumping && isFalling)
            {
                isJumping = false;
            }
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(origin, direction * 1.1f,
                     isGrounded ? Color.green : Color.red);
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
    
    public void Jump()
    {
        if (isJumping || !isGrounded || isFalling)
        {
            return;
        }

        isJumping = true;
        if (rb.velocity.y != 0f)
        {
            rb.velocity = Vector3.zero;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
