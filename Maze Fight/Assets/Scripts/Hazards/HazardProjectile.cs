using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class HazardProjectile : MonoBehaviour
{
    Rigidbody rb;

    public GameObject ProjectileDestroyEffect;

    public bool IsPlayerProjectile = false;
    public LayerMask PlayerLayer;
    public LayerMask EnemyLayer;

    public float ProjectileSpeed = 5f;
    Vector3 lastVelocity;
    public float ProjectileLifetime = 999f;
    float currentLifetime = 0f;
    public float ImpactShakeMagnitude = 1f;
    public float ImpactShakeRoughness = 1f;
    public float ImpactShakeFadeInTime = 0.25f;
    public float ImpactShakeFadeOutTime = 0.25f;
    
    public bool CanBounce = false;
    public int MaxBounces = 0;
    int currentBounces = 0;
    Vector3 normal;

    public bool CanReturn = false;
    public float ReturnTime = 0f;
    bool dontDestroyOnContact = false;

    public bool IsAreaOfEffect = false;
    public float AOERadius = 0.5f;
    public float AOEDamageModifier = 1f;
    public float AOEShakeMagnitude = 2f;
    public float AOEShakeRoughness = 2f;
    public float AOEShakeFadeInTime = 0.5f;
    public float AOEShakeFadeOutTime = 0.5f;

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

        if (IsAreaOfEffect)
        {
            DamageSurroundingArea();
        }

        Destroy(this.gameObject);
    }

    void DamageSurroundingArea()
    {
        HealthAndDamage had;

        // if we are the player, check the enemy layer
        LayerMask mask = IsPlayerProjectile ? EnemyLayer : PlayerLayer;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AOERadius, mask, QueryTriggerInteraction.Collide);

        if(hitColliders.Length > 0)
        {
            CameraShaker.Instance.ShakeOnce(ImpactShakeMagnitude, ImpactShakeRoughness, ImpactShakeFadeInTime, ImpactShakeFadeOutTime);
        }

        foreach (var hitCollider in hitColliders)
        {
            had = hitCollider.transform.GetComponentInParent<HealthAndDamage>();
            if (had)
                had.TakeDamage(Damage * AOEDamageModifier);
        }
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
            // TODO: code to hit either player or enemy.  If it is an AOE projectile then damage is handled in the DestroyProjectile function
            if((other.CompareTag("Player") || other.CompareTag("Enemy")) && !IsAreaOfEffect)
            {
                other.GetComponentInParent<HealthAndDamage>().TakeDamage(Damage);
                CameraShaker.Instance.ShakeOnce(AOEShakeMagnitude, AOEShakeRoughness, AOEShakeFadeInTime, AOEShakeFadeOutTime);
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
