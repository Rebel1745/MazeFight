using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthAndDamage : MonoBehaviour
{
    public PopupNumbers pn;

    public bool isPlayer = true;
    public bool ShowHealText = true;

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
    }

    void UpdateHealthBar(bool stealFocus)
    {
        if (!healthBar)
            return;

        if (!isPlayer)
        {            
            healthBar.SetMaxHealth(StartingHealth);
            //if (stealFocus)
            //{
                Debug.Log("Steal");
                healthBarGO.SetActive(true);
                healthBar.SetVisibleTimer(HealthBarVisibilityTimer);
            //}
        }

        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar(true);

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

        UpdateHealthBar(false);

        if(ShowHealText)
            pn.CreatePopup("+" + healAmount.ToString(), HealColour);
    }

    public void HealPercent(float amount)
    {
        int addedHealth = Mathf.RoundToInt(StartingHealth * amount);
        currentHealth += addedHealth;

        if (currentHealth > StartingHealth)
            currentHealth = StartingHealth;

        UpdateHealthBar(false);

        if(ShowHealText)
            pn.CreatePopup("+" + addedHealth.ToString(), HealColour);
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
