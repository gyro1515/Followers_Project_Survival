using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBase : MonoBehaviour
{
    protected Vector2 moveInput;
    public Vector2 MoveInput { get { return moveInput; } }

    protected virtual void Awake()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
