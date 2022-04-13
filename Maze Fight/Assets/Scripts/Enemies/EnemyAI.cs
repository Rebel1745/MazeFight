using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    public LayerMask whatIsPlayer;

    // Moving
    public float MoveSpeed = 1f;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Animator anim;

    public string animState;

    internal string IDLE = "Z_Idle";
    internal string WALK = "Z_Walk_InPlace";
    internal string ATTACK = "Z_Attack";

    private void Start()
    {
        ChangeAnimationState(IDLE);
    }

    private void Update()
    {
        CheckRanges();
    }

    void CheckRanges()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Idle();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Idle()
    {
        ChangeAnimationState(IDLE);
    }

    private void ChasePlayer()
    {
        transform.LookAt(player);
        Vector3 newPos = Vector3.MoveTowards(transform.position, player.position, MoveSpeed);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        ChangeAnimationState(WALK);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            ChangeAnimationState(ATTACK);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void ChangeAnimationState(string newState)
    {
        if (animState == newState)
            return;

        anim.Play(newState);

        animState = newState;
    }
}
