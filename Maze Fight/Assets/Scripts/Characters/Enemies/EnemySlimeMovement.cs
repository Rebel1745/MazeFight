using System;
using UnityEngine;

public class EnemySlimeMovement : MonoBehaviour
{
    public CharacterMovement characterMovement;

    public Transform Player;

    private bool canJump = true;
    private bool isJumping = false;
    public float JumpHeight = 1f;
    public float JumpDuration = 1f;
    public float JumpDistance = 1f;
    private float currentJumpTime = 0f;
    public float TimeBetweenJumps = 1f;
    private Vector3 jumpStartPos = Vector3.zero;
    private Vector3 jumpEndPos = Vector3.zero;

    private void Start()
    {
        canJump = true;
        isJumping = false;
    }

    private void Update()
    {
        if (!Player)
        {
            FindPlayer();
            return;
        }
        CheckJumping();
    }

    void FindPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            Player = null;
    }

    public void Jump()
    {
        // if we cant move, we cant initiate a jump
        if (!characterMovement.CanMove)
            return;

        transform.LookAt(Player);

        Vector3 jumpTarget = transform.forward * JumpDistance;
        currentJumpTime = 0;
        isJumping = true;
        canJump = false;

        jumpStartPos = transform.position;
        jumpEndPos = transform.position + jumpTarget;
    }

    void CheckJumping()
    {
        if (!canJump && !isJumping)
            return;
        
        if (!isJumping && canJump)
            Jump();

        currentJumpTime += Time.deltaTime;

        currentJumpTime = currentJumpTime % JumpDuration;

        if (currentJumpTime >= JumpDuration / 2)
        {
            // landing animation
        }
        else
        {
            // jumping animation
        }

        transform.position = Parabola(jumpStartPos, jumpEndPos, JumpHeight, currentJumpTime / JumpDuration);

        if ((JumpDuration - currentJumpTime) <= 0.05f)
        {
            isJumping = false;
            Invoke("ResetJump", TimeBetweenJumps);
        }
    }

    void ResetJump()
    {
        canJump = true;
    }

    public Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}

