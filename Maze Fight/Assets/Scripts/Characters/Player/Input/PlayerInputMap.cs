using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerInputMap : MonoBehaviour
{
    bool isMapVisible = true;

    GameObject fullMap;
    GameObject miniMap;

    MazeGenerator mg;

    void Start()
    {
        mg = FindObjectOfType<MazeGenerator>();
        fullMap = mg.FullMap;
        miniMap = mg.MiniMap;

        HideMap();
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isMapVisible)
            {
                HideMap();
            }
            else
            {
                ShowMap();
            }
        }        
    }

    void ShowMap()
    {
        // map is not visible, open it
        fullMap.SetActive(true);
        miniMap.SetActive(false);
        isMapVisible = true;
    }

    void HideMap()
    {
        // map is visible, close it
        fullMap.SetActive(false);
        miniMap.SetActive(true);
        isMapVisible = false;
    }
}
