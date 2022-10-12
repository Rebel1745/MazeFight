using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHealth : MonoBehaviour
{
    public float HealthAmount = 5f;

    private void OnTriggerEnter(Collider other)    
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponentInParent<HealthAndDamage>().Heal(HealthAmount);

            Destroy(transform.parent.gameObject);
        }
    }
}
