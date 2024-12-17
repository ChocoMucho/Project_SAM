using System;
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
    public bool run;
    public bool fire;
    public bool reload;

    #region receipt value
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnJump(InputValue value) => JumpInput(value.isPressed);
    public void OnAim(InputValue value) => AimInput(value.isPressed);
    public void OnRun(InputValue value) => RunInput(value.isPressed);
    public void OnFire(InputValue value) => FireInput(value.isPressed);
    public void OnReload(InputValue value) => ReloadInput(value.isPressed);
    #endregion

    #region restore value
    private void MoveInput(Vector2 moveDirection) => move = moveDirection;
    private void LookInput(Vector2 lookDirection) => look = lookDirection; 
    private void JumpInput(bool isPressed) => jump = isPressed;
    private void AimInput(bool isPressed) => aim = isPressed;
    private void RunInput(bool isPressed) => run = isPressed;
    private void FireInput(bool isPressed) => fire = isPressed;
    private void ReloadInput(bool isPressed) => reload = isPressed;
    #endregion
}
