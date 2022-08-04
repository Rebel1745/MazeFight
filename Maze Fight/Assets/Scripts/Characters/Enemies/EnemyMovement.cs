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

    // Attacks (general)
    public bool IsMeleeAttacker = false;
    public float MeleeAttackDistance = 0.5f;
    float MeleeAttackAnimDuration;
    public bool IsRangedAttacker = false;
    public float RangedAttackDistance = 1.5f;
    float RangedAttackAnimDuration;
    float attackDistance;
    string attackType;
    float attackAnimDuration;

    // Melee Attack
    [SerializeField] public float MeleeAttackDamage = 1f;
    [SerializeField] public float MeleeAttackRange;
    [SerializeField] public float AttackWidth = 0.001f;
    [SerializeField] public float MeleeAttackCooldown = 1f;

    // Ranged Attack
    [SerializeField] public GameObject RangedProjectilePrefab;
    [SerializeField] public Transform ProjectileSpawnPoint;
    [SerializeField] public float ProjectileSpeedMultiplier = 1f;
    [SerializeField] public float ProjectileDamage = 1f;
    [SerializeField] public float RangedAttackCooldown = 1f;

    public float ChasePlayerAfterAttack = 1f;

    float currentAttackTime;
    float currentCooldown;

    bool followPlayer, attackPlayer;

    private void Start()
    {
        SetAnimClipTimes();
        followPlayer = true;
        ChangeAnimationState(IDLE);

        // check to see if we can do both melee and ranged but we haven;t set the both flag
        // TODO: Change this to an Enum?
        /*if (IsMeleeAttacker && IsRangedAttacker && !IsMeleeAndRanged)
            IsMeleeAndRanged = true;*/

        SetAttackParams();
    }

    private void Update()
    {
        if (!Player)
        {
            FindPlayer();

        }
        else
        {
            if (!characterMovement.CanMove)
                return;

            FollowPlayer();
            AttackPlayer();

        }
    }

    void FindPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            Player = null;
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
            currentCooldown = MeleeAttackCooldown;
        }
        else
        {
            attackDistance = RangedAttackDistance;
            attackType = RANGEDATTACK;
            attackAnimDuration = RangedAttackAnimDuration;
            currentCooldown = RangedAttackCooldown;
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

    // TODO: making this a trigger from animation event will allow it to be only called once
    // removing the current issue of one attack destroying a player
    void MeleeAttack()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, AttackWidth, characterMovement.LastLookDirection, out hit, MeleeAttackRange, WhatIsPlayer))
        {
            //Debug.Log("Hit " + hit.transform.name);
            // Deal some damage
            hit.transform.gameObject.GetComponent<HealthAndDamage>().TakeDamage(MeleeAttackDamage);
            // find the direction between the colliding objects
            Vector3 dir = hit.transform.position - transform.position;
            hit.transform.gameObject.GetComponent<Knockback>().KnockbackObject(dir, 0.1f);
            // TODO: SORT THIS. If attackPlayer is not made false, multiple projectiles are released.  If followPlayer is not true, the enemy just stands there idle and doesnt shoot again.
            // if code as below, multiple projectiles are released AND an error appears.

            //attackPlayer = false;
            //followPlayer = true;
        }
    }

    void RangedAttack()
    {
        // instantiate the prjectile and set it flying
        GameObject projectile = Instantiate(RangedProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity);
        HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
        hp.Damage = ProjectileDamage;
        hp.IsPlayerProjectile = false;
        hp.SetVelocity(ProjectileSpawnPoint.forward * ProjectileSpeedMultiplier);
        // TODO: SORT THIS. If attackPlayer is not made false, multiple projectiles are released.  If followPlayer is not true, the enemy just stands there idle and doesnt shoot again.
        // if code as below, multiple projectiles are released AND an error appears.

        //attackPlayer = false;
        //followPlayer = true;
    }

    // TODO: Rewrite all enemy AI logic to allow for idle, following and attacking to take into account types of enemy (Melee/ranged)
    void AttackPlayer()
    {
        if (!attackPlayer)
            return;

        currentAttackTime += Time.deltaTime;

        if(currentAttackTime > currentCooldown)
        {
            ChangeAnimationState(attackType);
            if (IsMeleeAttacker)
            {
                MeleeAttack();
            }
            else { 
                // move this next line to an AnimationEvent.  TESTING PURPOSES ONLY
                RangedAttack();
            }            
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
