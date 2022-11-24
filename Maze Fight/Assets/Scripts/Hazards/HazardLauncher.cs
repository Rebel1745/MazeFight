using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLauncher : MonoBehaviour
{
    public GameObject Projectile;
    public Transform[] ProjectileSpawnPoints;
    public float ProjectileSpeed = 1f;
    public float ProjectileLifetime = 3f;
    public float TimeBetweenShots = 1f;
    public int ShotsPerBurst = 1;
    int currentShotCount;
    public float TimeBetweenBursts = 1f;
    float currentTimeBetweenShots;
    float currentTimeBetweenBursts;

    private void Start()
    {
        currentShotCount = 0;
        currentTimeBetweenBursts = TimeBetweenBursts;
        currentTimeBetweenShots = 0f;
    }

    private void Update()
    {
        TimeToShoot();
    }

    void TimeToShoot()
    {
        currentTimeBetweenBursts += Time.deltaTime;

        if(currentTimeBetweenBursts >= TimeBetweenBursts)
        {
            // we should be shooting, currentTimeBetweenBursts should only get reset when shooting is done
            if(currentShotCount < ShotsPerBurst)
            {
                // check to see if we are ready to shoot
                currentTimeBetweenShots += Time.deltaTime;
                if(currentTimeBetweenShots >= TimeBetweenShots)
                {
                    Shoot();
                    currentTimeBetweenShots = 0f;
                    currentShotCount++;
                }
            }
            else
            {
                currentTimeBetweenBursts = 0f;
                currentShotCount = 0;
            }
        }
    }

    void Shoot()
    {
        // Don't know why this IF is required.  Even though the projectile spawns without it, it still errors. Weird
        if (ProjectileSpawnPoints.Length > 0)
        {
            foreach(Transform t in ProjectileSpawnPoints)
            {
                GameObject projectile = Instantiate(Projectile, t.position, t.rotation);
                HazardProjectile hp = projectile.GetComponent<HazardProjectile>();
                hp.ProjectileSpeed = ProjectileSpeed;
                hp.ProjectileLifetime = ProjectileLifetime;
                hp.CanBounce = false;
                hp.MaxBounces = 0;
                hp.IsPlayerProjectile = false;
            }
        }
        
    }
}
