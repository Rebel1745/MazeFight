using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventRedirector : MonoBehaviour
{
    EnemyMovement em;

    private void Start()
    {
        em = GetComponentInParent<EnemyMovement>();
    }

    public void MeleeAttack()
    {
        em.MeleeAttack();
    }

    public void RangedAttack()
    {
        em.RangedAttack();
    }
}
