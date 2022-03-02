using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorController : MonoBehaviour
{
    public MazeGenerator mg;
    public PlayerInputMovement pm;
    public Transform DoorCheck;
    public LayerMask WhatIsDoor;
    public float DoorCheckDistance = 1f;
    GameObject currentDoor;

    private void Start()
    {
        mg = FindObjectOfType<MazeGenerator>();
    }

    void Update()
    {
        UpdateCurrentDoor();
    }

    public void OpenDoor()
    {
        if (currentDoor)
        {
            int nextRoom;
            if (pm.CurrentCell.CellNumber == currentDoor.GetComponent<Door>().DoorToCellNo1)
                nextRoom = currentDoor.GetComponent<Door>().DoorToCellNo2;
            else
                nextRoom = currentDoor.GetComponent<Door>().DoorToCellNo1;

            mg.ActivateRoom(nextRoom);
            Destroy(currentDoor);
            currentDoor = null;
        }
    }

    void UpdateCurrentDoor()
    {
        if (Physics.Raycast(DoorCheck.position, DoorCheck.transform.forward, out RaycastHit hit, DoorCheckDistance, WhatIsDoor) && hit.transform.gameObject != currentDoor)
        {
            currentDoor = hit.transform.gameObject;
        }
        else
        {
            currentDoor = null;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(DoorCheck.position, DoorCheck.transform.forward * DoorCheckDistance, Color.red);
    }
}
