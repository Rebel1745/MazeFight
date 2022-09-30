using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private bool setToHideAfterTime = false;
    private float visibleTime;
    private float currentVisibleTime = 0f;

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
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
