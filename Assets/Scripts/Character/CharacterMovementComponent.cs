using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 실제 로직 구현
public class CharacterMovementComponent : MonoBehaviour
{
    public float walkSpeed;
    public float jumpForce = 5f;
    public LayerMask groundDetectLayerMask;

    float lastJumpTime;
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
        Vector3 origin = transform.position;
        origin.y += 0.96f;

        var hits = Physics.SphereCastAll(transform.position, 0.4f, Vector3.up, 0f, groundDetectLayerMask);
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Ground");
        
        
        if (Physics.SphereCast(origin, 0.4f, Vector3.down, out hit, 1f, groundDetectLayerMask))
        {
            isGrounded = true;
            if (Time.time - lastJumpTime > 0.1f)
            {
                if (isJumping)
                {
                    isJumping = false;
                }
            }
            
        }
        else
        {
            isGrounded = false;
        }

        
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

        lastJumpTime = Time.time;
        isJumping = true;
        if (rb.velocity.y != 0f)
        {
            rb.velocity = Vector3.zero;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    
}
