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
        if (context.performed && attackLeftAvailable)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_LEFT);
            isAttacking = true;
            attackLeftAvailable = false;
            attackLeftCooldown = AttackLeftCooldown;
        }
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (context.performed && attackRightAvailable)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_PUNCH_RIGHT);
            isAttacking = true;
            attackRightAvailable = false;
            attackRightCooldown = AttackRightCooldown;
        }
    }

    public void AttackRanged(InputAction.CallbackContext context)
    {
        if (context.performed && attackRangedAvailable)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_RANGED);
            isAttacking = true;
            attackRangedAvailable = false;
            attackRangedCooldown = AttackRangedCooldown;
        }
    }

    public void AttackSpin(InputAction.CallbackContext context)
    {
        if (context.performed && attackSpinAvailable)
        {
            playerController.ChangeAnimationState(playerController.PLAYER_ATTACK_SPIN_FISTS);
            isAttacking = true;
            attackSpinAvailable = false;
            attackSpinCooldown = AttackSpinCooldown;
        }
    }

    void StopAttacking()
    {
        isAttacking = false;
    }

    #endregion
}
