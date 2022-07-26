using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
            Debug.LogError("No RigidBody found");

        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogError("Animator not found");

        pc = new PlayerInputActions();
    }

    private void OnEnable()
    {
        pc.Enable();
    }

    private void OnDisable()
    {
        pc.Disable();
    }

    internal Rigidbody rb;
    internal PlayerInputActions pc;
    public Animator anim;

    [SerializeField] internal PlayerInputMovement playerInputMove;
    [SerializeField] internal PlayerInputAttack playerInputAttack;
    [SerializeField] internal CharacterMovement characterMovement;

    public string animState;

    // also used in PlayerInputAttack as strings
    internal string PLAYER_IDLE = "Idle";
    internal string PLAYER_WALK = "Walk";
    internal string PLAYER_TO_BALL = "TransitionToBall";
    internal string PLAYER_FROM_BALL = "TransitionFromBall";
    internal string PLAYER_ATTACK_PUNCH_RIGHT = "AttackPunchRight";
    internal string PLAYER_ATTACK_PUNCH_LEFT = "AttackPunchLeft";
    internal string PLAYER_ATTACK_RANGED = "AttackRanged";
    internal string PLAYER_ATTACK_SPIN_FISTS = "SpinFists";

    public void ChangeAnimationState(string newState, float speed = 1f)
    {
        if (animState == newState)
            return;

        anim.speed = speed;
        anim.Play(newState);

        animState = newState;
    }
}
