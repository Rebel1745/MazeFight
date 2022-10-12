using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthAndDamage : MonoBehaviour
{
    public bool isPlayer = true;

    public float StartingHealth = 2f;
    float currentHealth;

    public float DestroyPopupAfterTime = 1f;
    public Transform DamagePopupSpawnPoint;
    public GameObject DamagePopupPrefab;
    public Color DamageColour;
    public Color HealColour;
    
    private HealthBar healthBar;
    private GameObject healthBarGO;

    public float HealthBarVisibilityTimer = 1f;

    public GameObject CollectableGoldPrefab;
    public GameObject CollectableHealthPrefab;
    public int CollectableCount = 1;
    public float HealthChancePercentage = 10f;

    void Start()
    {
        currentHealth = StartingHealth;

        if (isPlayer)
        {
            healthBarGO = GameObject.Find("PlayerHealthBar");
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

    void CreateDamagePopup(float damage, Color col)
    {
        GameObject damagePopup = Instantiate(DamagePopupPrefab, DamagePopupSpawnPoint.position, Quaternion.identity);
        TextMeshProUGUI tmp = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.faceColor = col;

        tmp.text = damage.ToString();

        Destroy(damagePopup, DestroyPopupAfterTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        if(DamagePopupPrefab)
            CreateDamagePopup(damage, DamageColour);

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

        if (DamagePopupPrefab)
            CreateDamagePopup(healAmount, HealColour);
    }

    void Die()
    {
        if(CollectableGoldPrefab && CollectableCount > 0)
        {
            // spawn collectable(s)
            for (int i = 0; i < CollectableCount; i++)
            {
                Instantiate(CollectableGoldPrefab, DamagePopupSpawnPoint.position, Quaternion.identity);
            }
        }

        float rand = Random.Range(0f, 100f);
        if(rand <= HealthChancePercentage)
        {
            // spawn a health pickup
            Instantiate(CollectableHealthPrefab, DamagePopupSpawnPoint.position, Quaternion.identity);
        }

        // code here for playing death animation
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
