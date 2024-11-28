using System;
using JetBrains.Annotations;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
    [Header("ÀÎÇ² °ª")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool aim;

    //receipt value
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnJump(InputValue value) => JumpInput(value.isPressed);
    public void OnAim(InputValue value) => AimInput(value.isPressed);


    //restore value
    public void MoveInput(Vector2 moveDirection) => move = moveDirection; 
    public void LookInput(Vector2 lookDirection) => look = lookDirection; 
    public void JumpInput(bool isPressed) => jump = isPressed;
    private void AimInput(bool isPressed) => aim = isPressed;
}
