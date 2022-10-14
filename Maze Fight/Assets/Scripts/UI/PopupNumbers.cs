using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupNumbers : MonoBehaviour
{
    public float DestroyPopupAfterTime = 1f;
    public Transform PopupSpawnPoint;
    public GameObject PopupPrefab;

    public void CreatePopup(string popupText, Color col)
    {
        GameObject popup = Instantiate(PopupPrefab, PopupSpawnPoint.position, Quaternion.identity);
        TextMeshProUGUI tmp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        tmp.color = col;

        tmp.text = popupText;

        Destroy(popup, DestroyPopupAfterTime);
    }
}
