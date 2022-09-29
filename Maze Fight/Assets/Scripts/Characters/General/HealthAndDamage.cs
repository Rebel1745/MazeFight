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

    void Start()
    {
        currentHealth = StartingHealth;

        GameObject tmp;

        if (isPlayer)
        {
            tmp = GameObject.Find("PlayerHealthBar");
        }
        else
        {
            tmp = GameObject.Find("EnemyHealthBar");
        }

        if(tmp)
            healthBar = tmp.GetComponent<HealthBar>();

        if (healthBar)
        {
            healthBar.SetMaxHealth(StartingHealth);
        }
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

        if(healthBar)
            healthBar.SetHealth(currentHealth);

        if(DamagePopupPrefab)
            CreateDamagePopup(damage);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // code here for playing death animation
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
