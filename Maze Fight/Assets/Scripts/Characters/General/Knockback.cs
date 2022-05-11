using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public Rigidbody rb;

    public float KnockbackMultiplier = 1f;
    public float KnockbackDuration = 0.1f;
    public float KnockbackSpeed = 50f;

    public void KnockbackObject(Vector3 dir)
    {
        GetComponent<PlayerInputMovement>().DisableMovement();
        rb.velocity = dir * KnockbackSpeed * KnockbackMultiplier;

        Invoke("StopKnockback", KnockbackDuration);
    }

    void StopKnockback()
    {
        //stops the enemy, set to movement speed when enemies are able to move
        rb.velocity = Vector3.zero;
        GetComponent<PlayerInputMovement>().EnableMovement();
    }
}
