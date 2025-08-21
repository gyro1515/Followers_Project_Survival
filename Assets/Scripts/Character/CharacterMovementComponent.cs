using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 실제 로직 구현
public class CharacterMovementComponent : MonoBehaviour
{
    private float walkSpeed = 5f;
    public float jumpForce = 5f;
    public LayerMask groundDetectLayerMask;

    float lastJumpTime;
    Rigidbody rb;
    StatComponentBase stat;

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
        
        if (stat == null)
        {
            stat = GetComponent<StatComponentBase>();
        }
        
        
        StatValue moveSpeed = stat.GetStatValue(StatType.MoveSpeed);
        if (moveSpeed != null)
        {
            walkSpeed = moveSpeed.FinalValue;
        }


    }

    private void Start()
    {
        StatValue moveSpeed = stat.GetStatValue(StatType.MoveSpeed);
        if ( moveSpeed != null )
        {
            moveSpeed.OnValueChanged += MoveSpeed_OnValueChanged;
        }
    }

    private void MoveSpeed_OnValueChanged(StatChangedEventArgs eventArgs)
    {
        walkSpeed = eventArgs.Current;
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

        RaycastHit hit;
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

    public bool CanJump()
    {
        if (isJumping || !isGrounded || isFalling)
        {
            return false;
        }
        return true;
    }
    
    public void Jump()
    {
        if (!CanJump())
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
