using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    PlayerController playerController;

    
    protected override void Awake()
    {
        base.Awake();

        playerController = controller as PlayerController;

    }

    protected override void Update()
    {
        base.Update();

        animator.SetFloat(AnimParam.Forward, playerController.MoveInput.y);
        animator.SetFloat(AnimParam.Right, playerController.MoveInput.x);


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        
    }

    public override ControllerBase GetController()
    {
        return playerController;
    }

}
