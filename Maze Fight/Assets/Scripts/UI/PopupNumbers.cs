using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupNumbers : MonoBehaviour
{
    public float DestroyPopupAfterTime = 1f;
    public Transform PopupSpawnPoint;
    public GameObject PopupPrefab;
    public float MaxRandomXOffset = 0.5f;
    public float MaxRandomYOffset = 0f;
    public float MaxRandomZOffset = 0f;

    public void CreatePopup(string popupText, Color textColour, bool useBGColour, Color backgroundColour)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-MaxRandomXOffset, MaxRandomXOffset), Random.Range(-MaxRandomYOffset, MaxRandomYOffset), Random.Range(-MaxRandomZOffset, MaxRandomZOffset));
        GameObject popup = Instantiate(PopupPrefab, PopupSpawnPoint.position + randomOffset, Quaternion.identity);
        TextMeshProUGUI tmp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        SpriteRenderer popupBackground = popup.transform.GetChild(1).GetComponent<SpriteRenderer>();
        
        if (popupBackground)
        {
            if (useBGColour)
            {
                popupBackground.color = backgroundColour;
                // add random rotation TODO Fix rotation application
                popupBackground.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            }
            else
            {
                popupBackground.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        tmp.color = textColour;

        tmp.text = popupText;

        Destroy(popup, DestroyPopupAfterTime);
    }
}
