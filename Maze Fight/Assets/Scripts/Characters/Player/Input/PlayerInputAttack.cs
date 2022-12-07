using UnityEngine;
using UnityEngine.InputSystem;
using EZCameraShake;

public class PlayerInputAttack : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    public ResourceAndUsage rau;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] public LayerMask WhatIsEnemy;
    [SerializeField] AudioSource source;

    [Header("Melee Attack")]
    public Transform MeleeAttackOriginPoint;
    public Transform MeleeAttackOriginPointLeft;
    public Transform MeleeAttackOriginPointRight;
    [SerializeField] bool meleeAttackAvailable = false;
    [SerializeField] public float MeleeAttackCooldown = 1f;
    float lastMeleeAttackCooldown;
    float meleeAttackCooldown = 0f;
    float meleeAttackAnimationSpeed = 1f;
    [SerializeField] public AudioClip MeleeWhoosh;
    [SerializeField] public float MeleeAttackDamage = 1f;
    // new range info
    public Transform MeleeAttackRightEndPoint;
    public Transform MeleeAttackLeftEndPoint;
    Transform currentStartPoint;
    Transform currentEndPoint;
    float attackRange;
    [SerializeField] public float AttackWidth = 0.001f;
    public int MaxEnemiesToHit = 10;
    private RaycastHit[] hits;

    public int ResourceGeneratedPerHit = 10;
    public float MeleeShakeMagnitude = 1f;
    public float MeleeShakeRoughness = 1f;
    public float MeleeShakeFadeInTime = 0.25f;
    public float MeleeShakeFadeOutTime = 0.25f;

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
    public int AttackSpinResourceCost = 50;
    public float SpinShakeMagnitude = 1f;
    public float SpinShakeRoughness = 1f;
    public float SpinShakeFadeInTime = 0.25f;
    public float SpinShakeFadeOutTime = 0.25f;

    [Header("Ranged Attack")]
    [SerializeField] bool attackRangedAvailable = false;
    [SerializeField] public float AttackRangedCooldown = 1f;
    float attackRangedCooldown = 0f;
    [SerializeField] public float AttackRangedAnimationSpeed = 1f;
    [SerializeField] public GameObject RangedProjectilePrefab;
    [SerializeField] public Transform ProjectileSpawnPoint;
    [SerializeField] public float ProjectileSpeed = 1f;
    [SerializeField] public float ProjectileSpeedMultiplier = 1f;
    [SerializeField] public float ProjectileDamage = 1f;
    public int AttackRangedResourceCost = 20;

    [Header("Appendage Scalling")]
    [SerializeField] public Transform UpperArmRight;
    [SerializeField] public Transform FistRight;
    [SerializeField] public Transform UpperArmLeft;
    [SerializeField] public Transform FistLeft;
    [SerializeField] public Vector3 UpperArmMeleeScale = new Vector3(1f, 1f, 1f);
    [SerializeField] public Vector3 FistMeleeScale = new Vector3(1f, 1f, 1f);
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
        hits = new RaycastHit[MaxEnemiesToHit];

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
                FistLeft.localScale = FistMeleeScale;
                break;
            case "FistRightPunch":
                FistRight.localScale = FistMeleeScale;
                break;
            case "UpperArmLeftPunch":
                UpperArmLeft.localScale = UpperArmMeleeScale;
                break;
            case "UpperArmRightPunch":
                UpperArmRight.localScale = UpperArmMeleeScale;
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
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT, meleeAttackAnimationSpeed);
            currentStartPoint = MeleeAttackOriginPointLeft;
            currentEndPoint = MeleeAttackLeftEndPoint;
        }
        else
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT, meleeAttackAnimationSpeed);
            currentStartPoint = MeleeAttackOriginPointRight;
            currentEndPoint = MeleeAttackRightEndPoint;
        }

        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(MeleeWhoosh);

        isAttacking = true;
        meleeAttackAvailable = false;
        meleeAttackCooldown = MeleeAttackCooldown;
        CancelAttackAfterAnimation(meleeAttackAnimationDuration / meleeAttackAnimationSpeed);

        // hits are now triggered in the animation 
        //CheckForMeleeHit();
    }
    
    public void CheckForMeleeHit()
    {
        // calculate the range of the attack from the melee attack origin to the end point marker of either the left or right punch
        attackRange = Vector3.Distance(currentStartPoint.position, currentEndPoint.position);
        
        // first fire out a short ray, this will check if there is anything directly infront of the player
        RaycastHit hit;
        if (Physics.Raycast(MeleeAttackOriginPoint.position, playerController.characterMovement.LastLookDirection, out hit, AttackWidth, WhatIsEnemy))
        {
            ProcessMeleeHit(hit);
        }
        else
        {
            if (Physics.Raycast(MeleeAttackOriginPointLeft.position, playerController.characterMovement.LastLookDirection, out hit, AttackWidth, WhatIsEnemy))
            {
                ProcessMeleeHit(hit);
                return;
            }
            else
            {
                if (Physics.Raycast(MeleeAttackOriginPointRight.position, playerController.characterMovement.LastLookDirection, out hit, AttackWidth, WhatIsEnemy))
                {
                    ProcessMeleeHit(hit);
                    return;
                }
                else
                {
                    System.Array.Clear(hits, 0, hits.Length);
                    // fire out a SphereCast corresponding to currentAttackRange, if it hits the enemy, hit it
                    if (Physics.SphereCastNonAlloc(MeleeAttackOriginPoint.position, AttackWidth, playerController.characterMovement.LastLookDirection.normalized, hits, attackRange - (AttackWidth / 2f), WhatIsEnemy) > 0)
                    {
                        //Debug.Log(hits.Length);
                        foreach (RaycastHit h in hits)
                        {
                            if(h.collider)
                                ProcessMeleeHit(h);
                        }
                    }
                }
            }            
        }
    }

    void ProcessMeleeHit(RaycastHit hit)
    {
        HealthAndDamage had = hit.transform.gameObject.GetComponent<HealthAndDamage>();

        // Deal some damage
        if (had && !had.IsInvincible)
        {
            had.TakeDamage(MeleeAttackDamage);

            // add some resource
            rau.AddResource(ResourceGeneratedPerHit);

            CameraShaker.Instance.ShakeOnce(MeleeShakeMagnitude, MeleeShakeRoughness, MeleeShakeFadeInTime, MeleeShakeFadeOutTime);
        }

        Knockback kb = hit.transform.gameObject.GetComponent<Knockback>();
        if (kb)
        {
            // find the direction between the colliding objects
            Vector3 dir = hit.transform.position - transform.position;
            kb.KnockbackObject(dir, 0.1f);
        }
    }

    #region Spinning
    public void AttackSpin(InputAction.CallbackContext context)
    {
        if (context.performed && attackSpinAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking && rau.CanAffordResourceCost(AttackSpinResourceCost))
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
        rau.UseResource(AttackSpinResourceCost);
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

        CheckSpinHit();
    }

    void CheckSpinHit()
    {
        float spinRadius = Vector3.Distance(MeleeAttackLeftEndPoint.position, MeleeAttackRightEndPoint.position) / 2f;
        
        // when we are spinning, check if any enemies are in our spin radius, and knock them back
        Collider[] cols = Physics.OverlapSphere(transform.position, spinRadius, WhatIsEnemy);
        
        if(cols.Length > 0)
        {
            CameraShaker.Instance.ShakeOnce(SpinShakeMagnitude, SpinShakeRoughness, SpinShakeFadeInTime, SpinShakeFadeOutTime);
        }

        foreach(Collider c in cols)
        {
            // the collider is on the GFX model, the code is on the top parent TODO: Make sure this is at the same level for every character
            Knockback kb = c.GetComponentInParent<Knockback>();
            if (kb)
            {
                // find the direction between the colliding objects
                Vector3 dir = c.transform.position - transform.position;
                kb.KnockbackObject(dir);
            }
        }
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
        if (context.performed && attackRangedAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking && rau.CanAffordResourceCost(AttackRangedResourceCost))
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
        GameObject projectile = Instantiate(RangedProjectilePrefab, ProjectileSpawnPoint.position, ProjectileSpawnPoint.rotation);
        HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
        hp.Damage = ProjectileDamage;
        hp.IsPlayerProjectile = true;
        hp.ProjectileSpeed = ProjectileSpeed * ProjectileSpeedMultiplier;
        rau.UseResource(AttackRangedResourceCost);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(MeleeAttackOriginPointLeft.position, playerController.characterMovement.LastLookDirection * attackRange);
        Gizmos.DrawRay(MeleeAttackOriginPointRight.position, playerController.characterMovement.LastLookDirection * attackRange);
        //Gizmos.DrawWireSphere(MeleeAttackOriginPoint.position, attackRange);
    }
}
