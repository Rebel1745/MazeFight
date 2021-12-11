using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorController : MonoBehaviour
{
    public Transform DoorCheck;
    public LayerMask WhatIsDoor;
    public float DoorCheckDistance = 1f;
    GameObject currentDoor;
    
    void Update()
    {
        UpdateCurrentDoor();
    }

    public void OpenDoor()
    {
        if (currentDoor)
        {
            Destroy(currentDoor);
            currentDoor = null;
        }
    }

    void UpdateCurrentDoor()
    {
        if (Physics.Raycast(DoorCheck.position, transform.forward, out RaycastHit hit, DoorCheckDistance, WhatIsDoor) && hit.transform.gameObject != currentDoor)
        {
            currentDoor = hit.transform.gameObject;
        }
        else
        {
            currentDoor = null;
        }
    }
}
