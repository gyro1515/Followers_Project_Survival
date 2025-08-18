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

    public override ControllerBase GetController()
    {
        return playerController;
    }

}
