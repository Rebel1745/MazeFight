using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI GoldText;

    public void UpdateGoldUI(int currentGold)
    {
        GoldText.text = currentGold.ToString();
    }
}
