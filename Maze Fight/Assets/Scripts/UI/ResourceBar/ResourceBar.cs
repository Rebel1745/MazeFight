using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI ResourceText;

    // TODO: make one slider script for this and health

    void SetResourceText()
    {
        if (ResourceText)
            ResourceText.text = Mathf.FloorToInt(slider.value) + " / " + slider.maxValue;
    }

    public void SetResource(float health)
    {
        slider.value = health;
        SetResourceText();
    }

    public void SetMaxResource(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        SetResourceText();
    }
}
