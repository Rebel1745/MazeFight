using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardProjectile : MonoBehaviour
{
    Rigidbody rb;

    public GameObject ProjectileDestroyEffect;

    public bool IsPlayerProjectile = false;

    public float ProjectileSpeed = 5f;
    Vector3 lastVelocity;
    public float ProjectileLifetime = 999f;
    float currentLifetime = 0f;
    
    public bool CanBounce = false;
    public int MaxBounces = 0;
    int currentBounces = 0;
    Vector3 normal;

    public bool CanReturn = false;
    public float ReturnTime = 0f;
    bool dontDestroyOnContact = false;

    public float Damage = 1f;

    // TODO: make the whole sound play even if the projectile is destroy quickly

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetVelocity(Vector3 vel)
    {
        rb.velocity = vel * ProjectileSpeed;
    }

    void Update()
    {
        CheckProjectileLifetime();
        lastVelocity = rb.velocity;

        if (CanReturn)
            CheckReturn();
    }

    void CheckReturn()
    {
        if(currentLifetime > ReturnTime)
        {
            CanReturn = false;
            dontDestroyOnContact = true;
            rb.velocity = -rb.velocity;
        }
    }

    void CheckProjectileLifetime()
    {
        currentLifetime += Time.deltaTime;

        if (currentLifetime >= ProjectileLifetime)
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
        // do nothing if it hits the player and they shot it
        if (IsPlayerProjectile && other.CompareTag("Player"))
            return;
        // do nothing if it hits an enemy and they shot it
        if (!IsPlayerProjectile && (other.CompareTag("Enemy") || other.CompareTag("Hazard")) )
            return;

        if (CanBounce && currentBounces < MaxBounces)
        {
            normal = -other.transform.forward;
            Bounce(normal);
        }
        else if (!other.CompareTag("Projectile") && !other.CompareTag("Hazard"))
        {
            // TODO: code to hit either player or enemy
            if(other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                other.GetComponentInParent<HealthAndDamage>().TakeDamage(Damage);
            }

            // if it can return, dont destry on contact
            if (!dontDestroyOnContact && !CanReturn)
            {
                DestroyProjectile();
            }
                
        }
    }

    void Bounce(Vector3 collisionNormal)
    {
        currentBounces++;
        float speed = lastVelocity.magnitude;
        Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collisionNormal);

        rb.velocity = direction * Mathf.Max(speed, ProjectileSpeed);
    }
}
