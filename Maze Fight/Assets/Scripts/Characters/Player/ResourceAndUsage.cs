using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAndUsage : MonoBehaviour
{
    public int MaxResource = 100;
    int currentResource;

    private ResourceBar resourceBar;
    private GameObject resourceBarGO;

    void Start()
    {
        currentResource = MaxResource;

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
        if (currentResource > MaxResource)
            currentResource = MaxResource;

        resourceBar.SetResource(currentResource);
    }

    public void AddResourcePercent(float amount)
    {
        AddResource(Mathf.RoundToInt(MaxResource * amount));
    }

    public void UseResource(int amount)
    {
        if (currentResource - amount < 0)
            Debug.LogError("Trying to spend too much. How was this not caught?");

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
