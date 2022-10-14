using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthAndDamage : MonoBehaviour
{
    public PopupNumbers pn;

    public bool isPlayer = true;

    public float StartingHealth = 2f;
    float currentHealth;

    public Color DamageColour;
    public Color HealColour;
    
    private HealthBar healthBar;
    private GameObject healthBarGO;

    public float HealthBarVisibilityTimer = 1f;

    public Transform CollectableSpawnPoint;
    public GameObject CollectableGoldPrefab;
    public GameObject CollectableHealthPrefab;
    public int CollectableCount = 1;
    public float HealthChancePercentage = 10f;

    void Start()
    {
        currentHealth = StartingHealth;

        if (isPlayer)
        {
            healthBarGO = GameObject.Find("PlayerStatusBar");
        }
        else
        {
            healthBarGO = GameObject.Find("EnemyHealthBar");
        }

        if (healthBarGO)
            healthBar = healthBarGO.GetComponent<HealthBar>();

        if (healthBar)
        {
            healthBar.SetMaxHealth(StartingHealth);
            if (!isPlayer)
                healthBar.SetVisibleTimer(0);
        }

        Debug.Log(DamageColour);
        Debug.Log(HealColour);
    }

    void UpdateHealthBar()
    {
        if (!healthBar)
            return;

        if (!isPlayer)
        {
            healthBarGO.SetActive(true);
            healthBar.SetMaxHealth(StartingHealth);
            healthBar.SetVisibleTimer(HealthBarVisibilityTimer);
        }

        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        pn.CreatePopup(damage.ToString(), DamageColour);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > StartingHealth)
            currentHealth = StartingHealth;

        UpdateHealthBar();

        pn.CreatePopup("+" + healAmount.ToString(), HealColour);
    }

    void Die()
    {
        if(CollectableGoldPrefab && CollectableCount > 0)
        {
            // spawn collectable(s)
            for (int i = 0; i < CollectableCount; i++)
            {
                Instantiate(CollectableGoldPrefab, CollectableSpawnPoint.position, Quaternion.identity);
            }
        }

        float rand = Random.Range(0f, 100f);
        if(rand <= HealthChancePercentage)
        {
            // spawn a health pickup
            Instantiate(CollectableHealthPrefab, CollectableSpawnPoint.position, Quaternion.identity);
        }

        // code here for playing death animation
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
