using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI HealthText;

    private bool setToHideAfterTime = false;
    private float visibleTime;
    private float currentVisibleTime = 0f;

    void SetHealthText()
    {
        if (HealthText)
            HealthText.text = Mathf.FloorToInt(slider.value) + " / " + slider.maxValue;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        SetHealthText();
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        SetHealthText();
    }

    public void SetVisibleTimer(float time)
    {
        currentVisibleTime = 0;
        setToHideAfterTime = true;
        visibleTime = time;
    }

    void CheckHideAfterTime()
    {
        if (currentVisibleTime >= visibleTime)
            gameObject.SetActive(false);
        currentVisibleTime += Time.deltaTime;
    }

    private void Update()
    {
        if (setToHideAfterTime)
            CheckHideAfterTime();
    }

}
