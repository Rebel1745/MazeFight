using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardProjectile : MonoBehaviour
{
    public GameObject ProjectileDestroyEffect;
    public float ProjectileLifetime = 999f;
    float currentDuration = 0f;
    public bool CanBounce = false;
    public int MaxBounces = 0;
    int currentBounces = 0;

    void Start()
    {

    }

    void Update()
    {
        CheckProjectileLifetime();
    }

    void CheckProjectileLifetime()
    {
        currentDuration += Time.deltaTime;

        if (currentDuration >= ProjectileLifetime)
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        if (ProjectileDestroyEffect)
        {
            Instantiate(ProjectileDestroyEffect, transform.position, transform.rotation);
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        /*if (!other.CompareTag("Player") && (!CanBounce || currentBounces > MaxBounces) && !other.CompareTag("Enemy"))
        {
            DestroyProjectile();
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            currentBounces++;
        }
    }
}
