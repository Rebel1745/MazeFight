using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAttack : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [Header("Attacks")]
    public bool isAttacking = false;

    bool attackRightAvailable = false;
    public float AttackRightCooldown = 1f;
    float attackRightCooldown = 0f;
    public float AttackRightAnimationSpeed = 1f;
    float lastAttackRightAnimationSpeed;

    bool attackLeftAvailable = false;
    public float AttackLeftCooldown = 1f;
    float attackLeftCooldown = 0f;
    public float AttackLeftAnimationSpeed = 1f;
    float lastAttackLeftAnimationSpeed;

    bool attackSpinAvailable = false;
    bool isSpinning = false;
    public float AttackSpinCooldown = 1f;
    float attackSpinCooldown = 0f;
    public int AttackSpinNumber = 1;
    float maxSpinDuration;
    float currentSpinDuration;

    bool attackRangedAvailable = false;
    public float AttackRangedCooldown = 1f;
    float attackRangedCooldown = 0f;
    public float AttackRangedAnimationSpeed = 1f;
    float lastAttackRangedAnimationSpeed;
    public GameObject RangedProjectilePrefab;
    public Transform ProjectileSpawnPoint;
    public float ProjectileSpeed = 1f;
    public float ProjectileSpeedMultiplier = 1f;
    public float ProjectileLifetime = 999f;

    float attackRightAnimationDuration, attackLeftAnimationDuration, attackRangedAnimationDuration, attackSpinAnimationDuration;

    private void Start()
    {
        SetAnimClipTimes();
    }

    void Update()
    {
        UpdateAttackCountdowns();
        CheckAnimationSpeedChanges();
        CheckSpinning();
    }

    void CheckAnimationSpeedChanges()
    {
        // if the animation speed for any of the attacks has changed, recalculate the durations
        if (
            AttackRightAnimationSpeed != lastAttackRightAnimationSpeed ||
            AttackLeftAnimationSpeed != lastAttackLeftAnimationSpeed ||
            AttackRangedAnimationSpeed != lastAttackRangedAnimationSpeed
            )
        {
            SetAnimClipTimes();

            lastAttackRightAnimationSpeed = AttackRightAnimationSpeed;
            lastAttackLeftAnimationSpeed = AttackLeftAnimationSpeed;
            lastAttackRangedAnimationSpeed = AttackRangedAnimationSpeed;
        }
    }

    #region Attacking
    void UpdateAttackCountdowns()
    {
        if (!attackRightAvailable)
            attackRightCooldown -= Time.deltaTime;

        if (attackRightCooldown <= 0f)
            attackRightAvailable = true;

        if (!attackLeftAvailable)
            attackLeftCooldown -= Time.deltaTime;

        if (attackLeftCooldown <= 0f)
            attackLeftAvailable = true;

        if (!attackRangedAvailable)
            attackRangedCooldown -= Time.deltaTime;

        if (attackRangedCooldown <= 0f)
            attackRangedAvailable = true;

        if (!attackSpinAvailable)
            attackSpinCooldown -= Time.deltaTime;

        if (attackSpinCooldown <= 0f)
            attackSpinAvailable = true;
    }

    public void AttackLeft(InputAction.CallbackContext context)
    {
        if (context.performed && attackLeftAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT, AttackLeftAnimationSpeed);
            isAttacking = true;
            attackLeftAvailable = false;
            attackLeftCooldown = AttackLeftCooldown / AttackLeftAnimationSpeed;
            CancelAttackAfterAnimation(attackLeftAnimationDuration);
        }
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (context.performed && attackRightAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT, AttackRightAnimationSpeed);
            isAttacking = true;
            attackRightAvailable = false;
            attackRightCooldown = AttackRightCooldown / AttackRightAnimationSpeed;
            CancelAttackAfterAnimation(attackRightAnimationDuration);
        }
    }

    public void AttackSpin(InputAction.CallbackContext context)
    {
        if (context.performed && attackSpinAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            StartSpinning();
        }
        else if (context.canceled && isSpinning)
        {
            float spinTimeRemaining = (maxSpinDuration - currentSpinDuration) % attackSpinAnimationDuration;
            Invoke("StopSpinning", spinTimeRemaining);
        }
    }

    void StartSpinning()
    {
        playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_SPIN_FISTS);
        maxSpinDuration = attackSpinAnimationDuration * AttackSpinNumber;
        isAttacking = true;
        isSpinning = true;
        currentSpinDuration = 0f;
        attackSpinAvailable = false;
    }

    void StopSpinning()
    {
        isSpinning = false;
        attackSpinCooldown = AttackSpinCooldown;
        StopAttacking();
    }

    void CheckSpinning()
    {
        if (!isSpinning)
            return;

        currentSpinDuration += Time.deltaTime;

        if (currentSpinDuration > maxSpinDuration)
            StopSpinning();
    }

    public void AttackRanged(InputAction.CallbackContext context)
    {
        if (context.performed && attackRangedAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_RANGED, AttackRangedAnimationSpeed);
            isAttacking = true;
            attackRangedAvailable = false;
            attackRangedCooldown = AttackRangedCooldown / AttackRangedAnimationSpeed;
            CancelAttackAfterAnimation(attackRangedAnimationDuration);
        }
    }

    public void FireRangedPrefab()
    {
        // instantiate the prjectile and set it flying
        GameObject projectile = Instantiate(RangedProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = ProjectileSpawnPoint.forward * ProjectileSpeed * ProjectileSpeedMultiplier;
        HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
        hp.ProjectileLifetime = ProjectileLifetime;
        hp.CanBounce = false;
        hp.MaxBounces = 0;
    }

    void CancelAttackAfterAnimation(float t)
    {
        Invoke("StopAttacking", t);
    }

    void StopAttacking()
    {
        isAttacking = false;
        playerController.ChangeAnimationState(playerController.PLAYER_IDLE);
    }

    public void SetAnimClipTimes()
    {
        // llop through the attack animations and get their duration.  If the animation is slower than the attack cooldown, change the cooldown
        AnimationClip[] clips = playerController.anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "AttackPunchRight":
                    attackRightAnimationDuration = clip.length / AttackRightAnimationSpeed;
                    if (AttackRightCooldown < attackRightAnimationDuration)
                        AttackRightCooldown = attackRightAnimationDuration;
                    break;
                case "AttackPunchLeft":
                    attackLeftAnimationDuration = clip.length / AttackLeftAnimationSpeed;
                    if (AttackLeftCooldown < attackLeftAnimationDuration)
                        AttackLeftCooldown = attackLeftAnimationDuration;
                    break;
                case "AttackRanged":
                    attackRangedAnimationDuration = clip.length / AttackRangedAnimationSpeed;
                    if (AttackRangedCooldown < attackRangedAnimationDuration)
                        AttackRangedCooldown = attackRangedAnimationDuration;
                    break;
                case "SpinFists":
                    attackSpinAnimationDuration = clip.length;
                    if (AttackSpinCooldown < attackSpinAnimationDuration)
                        AttackSpinCooldown = attackSpinAnimationDuration;
                    break;
            }
        }
    }

    #endregion
}
