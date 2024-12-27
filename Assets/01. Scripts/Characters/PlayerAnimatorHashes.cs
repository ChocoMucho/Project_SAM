using UnityEngine;

public static class PlayerAnimatorHashes
{
    // animation IDs
    public static readonly int Blend = Animator.StringToHash("Blend");
    public static readonly int Grounded = Animator.StringToHash("Grounded");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Fall = Animator.StringToHash("Fall");
    public static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int BlendX = Animator.StringToHash("BlendX");
    public static readonly int BlendY = Animator.StringToHash("BlendY");
    public static readonly int Aim = Animator.StringToHash("Aim");
    public static readonly int Fire = Animator.StringToHash("Fire");
    public static readonly int Reload = Animator.StringToHash("Reload");
    public static readonly int Dead = Animator.StringToHash("Dead");
    public static readonly int Run = Animator.StringToHash("Run");
}
