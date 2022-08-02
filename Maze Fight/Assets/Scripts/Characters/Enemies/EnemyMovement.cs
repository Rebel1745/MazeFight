using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Animator Anim;
    public CharacterMovement characterMovement;

    public string animState;

    public string IDLE = "EnemyIdle";
    public string WALK = "EnemyWalk";
    public string RANGEDATTACK = "EnemyAttackRanged";
    public string MELEEATTACK = "EnemyAttackPunchRight";

    public Rigidbody rb;

    public Transform Player;

    public LayerMask WhatIsPlayer;

    // Moving
    public float MoveSpeed = 1f;

    // Attacks
    public bool IsMeleeAttacker = false;
    public float MeleeAttackDistance = 0.5f;
    float MeleeAttackAnimDuration;
    public bool IsRangedAttacker = false;
    public float RangedAttackDistance = 1.5f;
    float RangedAttackAnimDuration;
    float attackDistance;
    string attackType;
    float attackAnimDuration;

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

        // check to see if we can do both melee and ranged but we haven;t set the both flag
        // TODO: Change this to an Enum?
        /*if (IsMeleeAttacker && IsRangedAttacker && !IsMeleeAndRanged)
            IsMeleeAndRanged = true;*/

        SetAttackParams();
    }

    private void Update()
    {
        if (!characterMovement.CanMove)
            return;

        FollowPlayer();
        AttackPlayer();
    }

    void SetAttackParams()
    {
        // TODO: optimise and add an option for both that checks for the correct attack to use
        // i.e. starting as a ranged character but if the player gets too close change to melee
        attackDistance = 0f;
        if (IsMeleeAttacker)
        {
            attackDistance = MeleeAttackDistance;
            attackType = MELEEATTACK;
            attackAnimDuration = MeleeAttackAnimDuration;
        }
        else
        {
            attackDistance = RangedAttackDistance;
            attackType = RANGEDATTACK;
            attackAnimDuration = RangedAttackAnimDuration;
        }            
    }

    void FollowPlayer()
    {
        if (!followPlayer)
            return;

        if(Vector3.Distance(transform.position, Player.position) > attackDistance)
        {
            transform.LookAt(Player);
            characterMovement.LastLookDirection = Player.position - transform.position;

            // Below doesn't want to work
            //rb.velocity = Vector3.right * MoveSpeed;

            Vector3 newPos = Vector3.MoveTowards(transform.position, Player.position, MoveSpeed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

            ChangeAnimationState(WALK);
        }
        else if(Vector3.Distance(transform.position, Player.position) <= attackDistance)
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
            ChangeAnimationState(attackType);
            Invoke(nameof(ResetAttack), attackAnimDuration);
        }

        if(Vector3.Distance(transform.position, Player.position) > attackDistance + ChasePlayerAfterAttack)
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

    public void SetAnimClipTimes()
    {
        // llop through the attack animations and get their duration.  If the animation is slower than the attack cooldown, change the cooldown
        AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "EnemyAttackPunchRight":
                    MeleeAttackAnimDuration = clip.length;
                    break;
                case "EnemyAttackRanged":
                    RangedAttackAnimDuration = clip.length;
                    break;
            }
        }
    }
}
