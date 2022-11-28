using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthAndDamage : MonoBehaviour
{
    public PopupNumbers pn;
    MazeGenerator mg;

    public bool isPlayer = true;
    public bool ShowHealText = true;

    public float StartingHealth = 2f;
    float currentHealth;

    public Color DamageColour;
    public Color DamageBGColour;
    public Color HealColour;
    
    private HealthBar healthBar;
    public GameObject healthBarGO;

    public float HealthBarVisibilityTimer = 1f;

    public float InvicibilityTimeAfterDamage = 0.5f;
    public bool IsInvincible = false;

    // collectables
    public Transform CollectableSpawnPoint;
    public GameObject CollectableGoldPrefab;
    public GameObject CollectableHealthPrefab;
    public int CollectableCount = 1;
    public float HealthChancePercentage = 10f;

    // Health Regen
    public bool IsHealthRegen = false;
    public float HealthRegenPerSecond = 2f;
    public float RegenDelayAfterDamage = 1f;
    float timeSinceLastHit = 0f;

    void Start()
    {
        mg = FindObjectOfType<MazeGenerator>();

        currentHealth = StartingHealth;

        if (isPlayer)
        {
            healthBarGO = mg.PlayerStatusBar;
        }
        else
        {
            healthBarGO = mg.EnemyHealthBar;
        }

        if (healthBarGO)
            healthBar = healthBarGO.GetComponent<HealthBar>();

        if (healthBar)
        {
            healthBar.SetMaxHealth(StartingHealth);
            if (!isPlayer)
                healthBar.SetVisibleTimer(0);
        }

        IsInvincible = false;
    }

    void Update()
    {
        if (IsHealthRegen)
            RegenerateHealth();
    }

    void RegenerateHealth()
    {
        timeSinceLastHit += Time.deltaTime;

        if(timeSinceLastHit > RegenDelayAfterDamage)
        {
            // only heal if we need to
            if(currentHealth < StartingHealth)
                Heal(HealthRegenPerSecond * Time.deltaTime);
        }
    }

    void UpdateHealthBar(bool stealFocus)
    {
        if (!healthBar)
            return;

        if (!isPlayer)
        {            
            healthBar.SetMaxHealth(StartingHealth);
            healthBarGO.SetActive(true);
            healthBar.SetVisibleTimer(HealthBarVisibilityTimer);
        }

        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsInvincible)
            return;

        timeSinceLastHit = 0f;
        MakeInvincible(InvicibilityTimeAfterDamage);

        currentHealth -= damage;

        UpdateHealthBar(true);

        pn.CreatePopup(damage.ToString(), DamageColour, true, DamageBGColour);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void MakeInvincible(float duration)
    {
        IsInvincible = true;
        // add invincibility animation (blinking or something)
        Invoke("MakeUnInvincible", duration);
    }

    void MakeUnInvincible()
    {
        IsInvincible = false;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > StartingHealth)
            currentHealth = StartingHealth;

        UpdateHealthBar(false);

        if(ShowHealText)
            pn.CreatePopup("+" + healAmount.ToString(), HealColour, false, Color.black);
    }

    public void HealPercent(float amount)
    {
        int addedHealth = Mathf.RoundToInt(StartingHealth * amount);
        currentHealth += addedHealth;

        if (currentHealth > StartingHealth)
            currentHealth = StartingHealth;

        UpdateHealthBar(false);

        if(ShowHealText)
            pn.CreatePopup("+" + addedHealth.ToString(), HealColour, false, Color.black);
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
