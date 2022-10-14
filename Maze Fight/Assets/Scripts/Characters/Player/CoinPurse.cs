using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinPurse : MonoBehaviour
{
    public PopupNumbers pn;

    public int StartingGold = 0;
    int currentGold;

    private GoldUI goldUI;
    private GameObject goldUIGO;

    public Color GoldColor;
    public Color PurchaseColor;

    // Start is called before the first frame update
    void Start()
    {
        goldUIGO = GameObject.Find("PlayerStatusBar");

        if (goldUIGO)
            goldUI = goldUIGO.GetComponent<GoldUI>();

        currentGold = StartingGold;
        goldUI.UpdateGoldUI(currentGold);
    }
    
    public void AddGold(int amount)
    {
        currentGold += amount;
        goldUI.UpdateGoldUI(currentGold);
        pn.CreatePopup(amount.ToString(), GoldColor);
    }

    public void SpendGold(int amount)
    {
        currentGold -= amount;
        goldUI.UpdateGoldUI(currentGold);
        pn.CreatePopup("-" + amount.ToString(), PurchaseColor);
    }
}
