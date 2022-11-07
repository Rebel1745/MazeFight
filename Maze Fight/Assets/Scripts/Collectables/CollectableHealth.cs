using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHealth : MonoBehaviour
{
    public bool IsPercentHeal = false;
    [Range(0,1)]
    public float PercentHealAmount = 0f;
    public float HealAmount = 5f;

    private void OnTriggerEnter(Collider other)    
    {
        if (other.transform.CompareTag("Player"))
        {
            HealthAndDamage had = other.transform.GetComponentInParent<HealthAndDamage>();
            if(IsPercentHeal)
                had.HealPercent(PercentHealAmount);
            else
                had.Heal(HealAmount);

            Destroy(transform.parent.gameObject);
        }
    }
}
