using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [Header("Input specs")]
    public UnityEvent changedInputToMouseAndKeyboard;
    public UnityEvent changedInputToGamepad;

    [HideInInspector]
    public Axis axisInput;
    [HideInInspector]
    public bool jump;

    private CharacterInputs movementInputs;

    void Awake()
    {
        movementInputs = new CharacterInputs();

        movementInputs.Gameplay.Move.performed += ctx => OnMove(ctx);
        movementInputs.Gameplay.Move.canceled += ctx => MoveEnded(ctx);

        movementInputs.Gameplay.Jump.performed += ctx => OnJump();
        movementInputs.Gameplay.Jump.canceled += ctx => JumpEnded();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        axisInput = ctx.ReadValue<Axis>();
    }

    public void MoveEnded(InputAction.CallbackContext ctx)
    {
        axisInput = 0;
    }

    public void OnJump()
    {
        jump = true;
        Debug.Log("test");
    }


    public void JumpEnded()
    {
        jump = false;
    }

}
