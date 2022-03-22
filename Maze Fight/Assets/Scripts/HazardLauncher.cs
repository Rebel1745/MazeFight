using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLauncher : MonoBehaviour
{
    public GameObject Projectile;
    public Transform ProjectileSpawnPoint;
    public float ProjectileSpeed = 1f;
    public float ProjectileLifetime = 3f;
    public float TimeBetweenShots = 1f;
    float currentTime = 0f;

    private void Start()
    {
        currentTime = TimeBetweenShots;
    }

    private void Update()
    {
        if(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            Shoot();
        }
    }

    void Shoot()
    {
        currentTime = TimeBetweenShots;

        // Don't know why this IF is required.  Even though the projectile spawns without it, it still errors. Weird
        if (ProjectileSpawnPoint)
        {
            GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().velocity = ProjectileSpawnPoint.right * ProjectileSpeed;
            HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
            hp.ProjectileLifetime = ProjectileLifetime;
            hp.CanBounce = true;
            hp.MaxBounces = Random.Range(0, 2);
        }
        
    }
}
