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
    bool attackLeftAvailable = false;
    public float AttackLeftCooldown = 1f;
    float attackLeftCooldown = 0f;
    bool attackRangedAvailable = false;
    public float AttackRangedCooldown = 1f;
    float attackRangedCooldown = 0f;
    bool attackSpinAvailable = false;
    public float AttackSpinCooldown = 1f;
    float attackSpinCooldown = 0f;

    public float attackRightDuration, attackLeftDuration, attackRangedDuration, attackSpinFistsDuration;

    private void Start()
    {
        SetAnimClipTimes();
    }

    void Update()
    {
        UpdateAttackCountdowns();
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
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT);
            isAttacking = true;
            attackLeftAvailable = false;
            attackLeftCooldown = AttackLeftCooldown;
            CancelAttackAfterAnimation(attackLeftDuration);
        }
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (context.performed && attackRightAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT);
            isAttacking = true;
            attackRightAvailable = false;
            attackRightCooldown = AttackRightCooldown;
            CancelAttackAfterAnimation(attackRightDuration);
        }
    }

    public void AttackRanged(InputAction.CallbackContext context)
    {
        if (context.performed && attackRangedAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_RANGED);
            isAttacking = true;
            attackRangedAvailable = false;
            attackRangedCooldown = AttackRangedCooldown;
            CancelAttackAfterAnimation(attackRangedDuration);
        }
    }

    public void AttackSpin(InputAction.CallbackContext context)
    {
        if (context.performed && attackSpinAvailable && playerController.playerInputMove.isBodyStandard && !isAttacking)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_SPIN_FISTS);
            isAttacking = true;
            attackSpinAvailable = false;
            attackSpinCooldown = AttackSpinCooldown;
            CancelAttackAfterAnimation(attackSpinFistsDuration);
        }
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
                    attackRightDuration = clip.length;
                    if (AttackRightCooldown < attackRightDuration)
                        AttackRightCooldown = attackRightDuration;
                    break;
                case "AttackPunchLeft":
                    attackLeftDuration = clip.length;
                    if (AttackLeftCooldown < attackLeftDuration)
                        AttackLeftCooldown = attackLeftDuration;
                    break;
                case "AttackRanged":
                    attackRangedDuration = clip.length;
                    if (AttackRangedCooldown < attackRangedDuration)
                        AttackRangedCooldown = attackRangedDuration;
                    break;
                case "SpinFists":
                    attackSpinFistsDuration = clip.length;
                    if (AttackSpinCooldown < attackSpinFistsDuration)
                        AttackSpinCooldown = attackSpinFistsDuration;
                    break;
            }
        }
    }

    #endregion
}
