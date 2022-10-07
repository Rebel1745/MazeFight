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
    
    private HealthBar healthBar;
    private GameObject healthBarGO;

    public float HealthBarVisibilityTimer = 1f;

    public GameObject CollectablePrefab;
    public int CollectableCount = 1;

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

    void CreateDamagePopup(float damage)
    {
        GameObject damagePopup = Instantiate(DamagePopupPrefab, DamagePopupSpawnPoint.position, Quaternion.identity);
        TextMeshProUGUI tmp = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        tmp.text = damage.ToString();

        Destroy(damagePopup, DestroyPopupAfterTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        if(DamagePopupPrefab)
            CreateDamagePopup(damage);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(CollectablePrefab && CollectableCount > 0)
        {
            // spawn collectable(s)
            for (int i = 0; i < CollectableCount; i++)
            {
                Instantiate(CollectablePrefab, DamagePopupSpawnPoint.position, Quaternion.identity);
            }
        }

        // code here for playing death animation
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
