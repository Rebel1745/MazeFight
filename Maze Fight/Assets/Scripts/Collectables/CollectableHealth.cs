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
            other.GetComponentInChildren<HealthAndDamage>().

            Destroy(transform.parent.gameObject);
        }
    }
}
