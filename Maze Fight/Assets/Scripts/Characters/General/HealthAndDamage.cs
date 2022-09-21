using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthAndDamage : MonoBehaviour
{
    public float StartingHealth = 2f;
    float currentHealth;

    public float DestroyPopupAfterTime = 1f;
    public Transform DamagePopupSpawnPoint;
    public GameObject DamagePopupPrefab;

    void Start()
    {
        currentHealth = StartingHealth;
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
