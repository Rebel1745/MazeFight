using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGold : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
