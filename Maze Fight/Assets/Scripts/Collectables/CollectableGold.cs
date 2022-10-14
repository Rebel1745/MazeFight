using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGold : MonoBehaviour
{
    public int GoldAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponentInParent<CoinPurse>().AddGold(GoldAmount);

            Destroy(transform.parent.gameObject);
        }
    }
}
