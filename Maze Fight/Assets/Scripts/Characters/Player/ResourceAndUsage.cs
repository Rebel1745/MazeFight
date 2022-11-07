using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAndUsage : MonoBehaviour
{
    public int StartingResource = 100;
    int currentResource;

    private ResourceBar resourceBar;
    private GameObject resourceBarGO;

    void Start()
    {
        currentResource = StartingResource;

        resourceBarGO = GameObject.Find("PlayerStatusBar");

        if (resourceBarGO)
            resourceBar = resourceBarGO.GetComponent<ResourceBar>();

        if (resourceBar)
        {
            resourceBar.SetMaxResource(currentResource);
        }
    }

    public void AddResource(int amount)
    {
        currentResource += amount;
        resourceBar.SetResource(currentResource);
    }

    public void AddResourcePercent(float amount)
    {
        currentResource += Mathf.RoundToInt(StartingResource * amount);
        resourceBar.SetResource(currentResource);
    }

    public void UseResource(int amount)
    {
        currentResource -= amount;
        resourceBar.SetResource(currentResource);
    }

    public bool CanAffordResourceCost(int amount)
    {
        if (currentResource - amount >= 0)
            return true;
        else
            return false;
    }
}
