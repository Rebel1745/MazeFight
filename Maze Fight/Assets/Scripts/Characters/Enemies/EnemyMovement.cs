using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Animator Anim;

    public string animState;

    internal string IDLE = "Z_Idle";
    internal string WALK = "Z_Walk";
    internal string ATTACK = "Z_Attack";

    float AttackAnimDuration;

    public Rigidbody rb;

    public Transform Player;

    public LayerMask WhatIsPlayer;

    // Moving
    public float MoveSpeed = 1f;
    bool canMove = true;

    public float AttackDistance = 1f;
    public float ChasePlayerAfterAttack = 1f;

    float currentAttackTime;
    public float DefaultAttackTime = 2;

    bool followPlayer, attackPlayer;

    private void Start()
    {
        SetAnimClipTimes();
        followPlayer = true;
        currentAttackTime = DefaultAttackTime;
        ChangeAnimationState(IDLE);
    }

    private void Update()
    {
        FollowPlayer();
        AttackPlayer();
    }

    void FollowPlayer()
    {
        if (!followPlayer)
            return;

        if(Vector3.Distance(transform.position, Player.position) > AttackDistance)
        {
            transform.LookAt(Player);

            // Below doesn't want to work
            //rb.velocity = Vector3.right * MoveSpeed;

            Vector3 newPos = Vector3.MoveTowards(transform.position, Player.position, MoveSpeed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

            ChangeAnimationState(WALK);
        }
        else if(Vector3.Distance(transform.position, Player.position) <= AttackDistance)
        {
            //rb.velocity = Vector3.zero;
            ChangeAnimationState(IDLE);
            followPlayer = false;
            attackPlayer = true;
        }
    }

    void AttackPlayer()
    {
        if (!attackPlayer)
            return;

        currentAttackTime += Time.deltaTime;

        if(currentAttackTime > DefaultAttackTime)
        {
            ChangeAnimationState(ATTACK);
            Invoke(nameof(ResetAttack), AttackAnimDuration);
        }

        if(Vector3.Distance(transform.position, Player.position) > AttackDistance + ChasePlayerAfterAttack)
        {
            attackPlayer = false;
            followPlayer = true;
        }
    }

    private void ResetAttack()
    {
        currentAttackTime = 0f;
        ChangeAnimationState(IDLE);
    }

    public void ChangeAnimationState(string newState)
    {
        if (animState == newState)
            return;

        Anim.StopPlayback();
        Anim.Play(newState);

        animState = newState;
    }

    public IEnumerator DisableMovementForTime(float time)
    {
        DisableMovement();
        yield return new WaitForSeconds(time);
        EnableMovement();
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void SetAnimClipTimes()
    {
        // llop through the attack animations and get their duration.  If the animation is slower than the attack cooldown, change the cooldown
        AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                // just look at AttackPunchRight not left, they are both the same
                case "Z_Attack":
                    AttackAnimDuration = clip.length;
                    break;
            }
        }
    }
}
