using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAttack : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] AudioSource source;

    [Header("Melee Attack")]
    [SerializeField] bool meleeAttackAvailable = false;
    [SerializeField] public float MeleeAttackCooldown = 1f;
    float lastMeleeAttackCooldown;
    float meleeAttackCooldown = 0f;
    float meleeAttackAnimationSpeed = 1f;
    [SerializeField] public AudioClip MeleeWhoosh;

    [Header("Spin Attack")]
    [SerializeField] bool attackSpinAvailable = false;
    [SerializeField] bool isSpinning = false;
    [SerializeField] public float AttackSpinCooldown = 1f;
    float attackSpinCooldown = 0f;
    [SerializeField] public int AttackSpinNumber = 1;
    float maxSpinDuration;
    float currentSpinDuration;
    [SerializeField] public float AttackSpinAnimationSpeed = 0.5f;
    [SerializeField] public AudioClip SpinWhoosh;

    [Header("Ranged Attack")]
    [SerializeField] bool attackRangedAvailable = false;
    [SerializeField] public float AttackRangedCooldown = 1f;
    float attackRangedCooldown = 0f;
    [SerializeField] public float AttackRangedAnimationSpeed = 1f;
    [SerializeField] public GameObject RangedProjectilePrefab;
    [SerializeField] public Transform ProjectileSpawnPoint;
    [SerializeField] public float ProjectileSpeedMultiplier = 1f;

    [Header("Appendage Scalling")]
    [SerializeField] public Transform UpperArmRight;
    [SerializeField] public Transform FistRight;
    [SerializeField] public Transform UpperArmLeft;
    [SerializeField] public Transform FistLeft;
    [SerializeField] public Vector3 UpperArmPunchScale = new Vector3(1f, 1f, 1f);
    [SerializeField] public Vector3 FistPunchScale = new Vector3(1f, 1f, 1f);
    [SerializeField] public Vector3 UpperArmSpinScale = new Vector3(1f, 1f, 1f);
    [SerializeField] public Vector3 FistSpinScale = new Vector3(1f, 1f, 1f);
    Vector3 upperArmRightInitialScale;
    Vector3 upperArmLeftInitialScale;
    Vector3 fistRightInitialScale;
    Vector3 fistLeftInitialScale;

    float meleeAttackAnimationDuration;
    float attackRangedAnimationDuration;
    float attackSpinAnimationDuration;

    private void Start()
    {
        SetAnimClipTimes();
        SetMeleeCooldownTime();
        SetInitialScales();
    }

    void Update()
    {
        UpdateAttackCountdowns();
        CheckAnimationSpeedChanges();
        CheckSpinning();
    }

    #region Appendage Scales
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
            case "FistLeftPunch":
                FistLeft.localScale = FistPunchScale;
                break;
            case "FistRightPunch":
                FistRight.localScale = FistPunchScale;
                break;
            case "UpperArmLeftPunch":
                UpperArmLeft.localScale = UpperArmPunchScale;
                break;
            case "UpperArmRightPunch":
                UpperArmRight.localScale = UpperArmPunchScale;
                break;
            case "Spin":
                FistLeft.localScale = FistSpinScale;
                FistRight.localScale = FistSpinScale;
                UpperArmLeft.localScale = UpperArmSpinScale;
                UpperArmRight.localScale = UpperArmSpinScale;
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
    #endregion

    void SetMeleeCooldownTime()
    {
        // if the cooldown time of the melee attack is less than the duration of the animation then change the animation speed
        if(MeleeAttackCooldown < meleeAttackAnimationDuration)
        {
            meleeAttackAnimationSpeed = meleeAttackAnimationDuration / MeleeAttackCooldown;
        }
        else
        {
            meleeAttackAnimationSpeed = 1f;
        }
    }

    void CheckAnimationSpeedChanges()
    {

        // if the melee attack cooldown has changed, recalculate the animation speed
        if(MeleeAttackCooldown != lastMeleeAttackCooldown)
        {
            SetMeleeCooldownTime();

            lastMeleeAttackCooldown = MeleeAttackCooldown;
        }
    }

    #region Attacking
    void UpdateAttackCountdowns()
    {
        if (!meleeAttackAvailable)
            meleeAttackCooldown -= Time.deltaTime;

        if (meleeAttackCooldown <= 0f)
            meleeAttackAvailable = true;

        if (!attackRangedAvailable)
            attackRangedCooldown -= Time.deltaTime;

        if (attackRangedCooldown <= 0f)
            attackRangedAvailable = true;

        if (!attackSpinAvailable && !isSpinning)
            attackSpinCooldown -= Time.deltaTime;

        if (attackSpinCooldown <= 0f)
            attackSpinAvailable = true;
    }

    public void AttackMelee(InputAction.CallbackContext context)
    {
        if (!context.performed || !meleeAttackAvailable || !playerController.playerInputMove.isBodyStandard || isAttacking)
            return;

        // pick a side and attack with it
        int rand = Random.Range(0, 2);
        if (rand == 0)
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT, meleeAttackAnimationSpeed);
        else
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT, meleeAttackAnimationSpeed);

        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(MeleeWhoosh);

        isAttacking = true;
        meleeAttackAvailable = false;
        meleeAttackCooldown = MeleeAttackCooldown;
        CancelAttackAfterAnimation(meleeAttackAnimationDuration / meleeAttackAnimationSpeed);
    }

    #region Spinning
    public void AttackSpin(InputAction.CallbackContext context)
    {
        if (context.performed && attackSpinAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            StartSpinning();
        }
        else if (context.canceled && isSpinning)
        {
            float spinTimeRemaining = (maxSpinDuration - currentSpinDuration) % (attackSpinAnimationDuration / AttackSpinAnimationSpeed);
            Invoke("StopSpinning", spinTimeRemaining);
        }
    }

    void StartSpinning()
    {
        playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_SPIN_FISTS, AttackSpinAnimationSpeed);
        UpdateAppendageScale("Spin");
        maxSpinDuration = (attackSpinAnimationDuration / AttackSpinAnimationSpeed) * AttackSpinNumber;
        isAttacking = true;
        isSpinning = true;
        currentSpinDuration = 0f;
        attackSpinAvailable = false;
        attackSpinCooldown = AttackSpinCooldown;
    }

    void StopSpinning()
    {
        isSpinning = false;
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

    public void PlaySpinSound()
    {
        source.pitch = 1f;
        source.PlayOneShot(SpinWhoosh);
    }
    #endregion

    #region Ranged
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
        HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
        hp.SetVelocity(ProjectileSpawnPoint.forward * ProjectileSpeedMultiplier);
    }
    #endregion

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
                // just look at AttackPunchRight not left, they are both the same
                case "AttackPunchRight":
                    meleeAttackAnimationDuration = clip.length;
                    break;
                case "AttackRanged":
                    attackRangedAnimationDuration = clip.length;
                    break;
                case "SpinFists":
                    attackSpinAnimationDuration = clip.length;
                    break;
            }
        }
    }

    #endregion
}
