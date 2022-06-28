using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargeted : MonoBehaviour
{
    public GameObject TargetedMark;

    private void Start()
    {
        RemoveTarget();
    }

    public void MarkAsTargeted()
    {
        TargetedMark.SetActive(true);
    }

    public void RemoveTarget()
    {
        TargetedMark.SetActive(false);
    }
}
