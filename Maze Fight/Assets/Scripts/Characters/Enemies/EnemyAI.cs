using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform Player;

    public LayerMask WhatIsPlayer;

    // Moving
    public float MoveSpeed = 1f;
    bool canMove = true;

    //Attacking
    public float TimeBetweenAttacks;
    bool alreadyAttacked = false;

    //States
    public float SightRange, AttackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Animator Anim;

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
        if (!canMove)
            return;

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatIsPlayer);

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
        transform.LookAt(Player);
        Vector3 newPos = Vector3.MoveTowards(transform.position, Player.position, MoveSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        ChangeAnimationState(WALK);
    }

    private void AttackPlayer()
    {
        transform.LookAt(Player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            ChangeAnimationState(ATTACK);
            PlayerInputMovement pim = Player.GetComponent<PlayerInputMovement>();
            Vector3 knockbackDir = pim.LastLookDirection;
            Player.GetComponent<Knockback>().KnockbackObject(-knockbackDir);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
