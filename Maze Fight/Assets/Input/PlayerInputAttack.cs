using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAttack : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] public bool isAttacking = false;

    [Header("Right Attack")]
    [SerializeField] bool attackRightAvailable = false;
    [SerializeField] public float AttackRightCooldown = 1f;
    float attackRightCooldown = 0f;
    [SerializeField] public float AttackRightAnimationSpeed = 1f;
    float lastAttackRightAnimationSpeed;

    [Header("Left Attack")]
    [SerializeField] bool attackLeftAvailable = false;
    [SerializeField] public float AttackLeftCooldown = 1f;
    float attackLeftCooldown = 0f;
    [SerializeField] public float AttackLeftAnimationSpeed = 1f;
    float lastAttackLeftAnimationSpeed;

    [Header("Spin Attack")]
    [SerializeField] bool attackSpinAvailable = false;
    [SerializeField] bool isSpinning = false;
    [SerializeField] public float AttackSpinCooldown = 1f;
    float attackSpinCooldown = 0f;
    [SerializeField] public int AttackSpinNumber = 1;
    float maxSpinDuration;
    float currentSpinDuration;

    [Header("Ranged Attack")]
    [SerializeField] bool attackRangedAvailable = false;
    [SerializeField] public float AttackRangedCooldown = 1f;
    float attackRangedCooldown = 0f;
    [SerializeField] public float AttackRangedAnimationSpeed = 1f;
    float lastAttackRangedAnimationSpeed;
    [SerializeField] public GameObject RangedProjectilePrefab;
    [SerializeField] public Transform ProjectileSpawnPoint;
    [SerializeField] public float ProjectileSpeed = 1f;
    [SerializeField] public float ProjectileSpeedMultiplier = 1f;
    [SerializeField] public float ProjectileLifetime = 999f;

    [Header("Appendage Scalling")]
    [SerializeField] public Transform UpperArmRight;
    [SerializeField] public Transform FistRight;
    [SerializeField] public Transform UpperArmLeft;
    [SerializeField] public Transform FistLeft;
    Vector3 upperArmRightInitialScale;
    Vector3 upperArmLeftInitialScale;
    Vector3 fistRightInitialScale;
    Vector3 fistLeftInitialScale;
    [SerializeField] public Vector3 UpperArmAttackScale = new Vector3(1f, 1f, 1f);
    [SerializeField] public Vector3 FistAttackScale = new Vector3(1f, 1f, 1f);

    float attackRightAnimationDuration, attackLeftAnimationDuration, attackRangedAnimationDuration, attackSpinAnimationDuration;

    private void Start()
    {
        SetAnimClipTimes();
        SetInitialScales();
    }

    void Update()
    {
        UpdateAttackCountdowns();
        CheckAnimationSpeedChanges();
        CheckSpinning();
    }

    void SetInitialScales()
    {
        upperArmLeftInitialScale = UpperArmLeft.localScale;
        upperArmRightInitialScale = UpperArmRight.localScale;

        fistLeftInitialScale = FistLeft.localScale;
        fistRightInitialScale = FistRight.localScale;
    }

    public void UpdateAppendageScale(string appendage)
    {
        switch (appendage)
        {
            case "FistLeft":
                FistLeft.localScale = FistAttackScale;
                break;
            case "FistRight":
                FistRight.localScale = FistAttackScale;
                break;
            case "UpperArmLeft":
                UpperArmLeft.localScale = UpperArmAttackScale;
                break;
            case "UpperArmRight":
                UpperArmRight.localScale = UpperArmAttackScale;
                break;
        }
    }

    void ResetAppendageScales()
    {
        UpperArmLeft.localScale = upperArmLeftInitialScale;
        UpperArmRight.localScale = upperArmRightInitialScale;
        FistLeft.localScale = fistLeftInitialScale;
        FistRight.localScale = fistRightInitialScale;
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

    public void AttackMelee(InputAction.CallbackContext context)
    {
        if (!context.performed || (!attackLeftAvailable && !attackRightAvailable) || !playerController.playerInputMove.isBodyStandard || isAttacking)
            return;

        // if both left and right attack are available, pick one, otherwise pick the side that is available
        if (attackLeftAvailable && attackRightAvailable)
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
                AttackLeft();
            else
                AttackRight();
        }
        else if (attackLeftAvailable)
            AttackLeft();
        else if (attackRightAvailable)
            AttackRight();
        else
            Debug.LogError("We should not be here, why are we attacking?");
    }

    void AttackLeft()
    {
        playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT, AttackLeftAnimationSpeed);
        isAttacking = true;
        attackLeftAvailable = false;
        attackLeftCooldown = AttackLeftCooldown / AttackLeftAnimationSpeed;
        CancelAttackAfterAnimation(attackLeftAnimationDuration);
    }

    void AttackRight()
    {
        playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT, AttackRightAnimationSpeed);
        isAttacking = true;
        attackRightAvailable = false;
        attackRightCooldown = AttackRightCooldown / AttackRightAnimationSpeed;
        CancelAttackAfterAnimation(attackRightAnimationDuration);
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
        ResetAppendageScales();
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
