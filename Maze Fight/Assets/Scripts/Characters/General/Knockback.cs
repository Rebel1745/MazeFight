using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public Rigidbody rb;
    public CharacterMovement characterMovement;

    // default knockback for the character
    public float KnockbackMultiplier = 1f;
    public float KnockbackDuration = 0.1f;
    public float KnockbackSpeed = 50f;

    // bonusKnockback refers to the extra amount imparted from the attack itself
    public void KnockbackObject(Vector3 dir, float bonusKnockback = 1f)
    {
        characterMovement.DisableMovement();
        rb.velocity = dir * KnockbackSpeed * KnockbackMultiplier * bonusKnockback;

        Invoke("StopKnockback", KnockbackDuration);
    }

    void StopKnockback()
    {
        //stops the enemy, set to movement speed when enemies are able to move
        rb.velocity = Vector3.zero;
        characterMovement.EnableMovement();
    }
}
