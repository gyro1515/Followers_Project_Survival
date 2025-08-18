using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected CharacterMovementComponent characterMovement;
    protected ControllerBase controller;
    protected Animator animator;

    protected virtual void Awake()
    {
        if (characterMovement == null)
        {
            characterMovement = GetComponent<CharacterMovementComponent>();
            if (characterMovement == null)
            {
                characterMovement = transform.AddComponent<CharacterMovementComponent>();
            }
        }

        if (controller == null)
        {
            controller = GetComponent<ControllerBase>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        characterMovement.Move(controller.MoveInput);
    }

    public virtual ControllerBase GetController()
    {
        return controller;
    }
}
