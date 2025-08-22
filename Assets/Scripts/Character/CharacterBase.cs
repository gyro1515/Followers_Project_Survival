using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    protected CharacterMovementComponent characterMovement;
    protected ControllerBase controller;
    protected Animator animator;
    protected StatComponentBase stat;

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

        if (stat == null)
        {
            stat = GetComponent<StatComponentBase>();
        }

    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        characterMovement.Move(controller.MoveInput);
    }

    public T GetController<T>() where T : ControllerBase
    {
        return controller as T;
    }

    public T GetStatComponent<T>() where T : StatComponentBase
    {
        return stat as T;
    }
}
