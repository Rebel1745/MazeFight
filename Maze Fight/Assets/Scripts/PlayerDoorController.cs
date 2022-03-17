using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorController : MonoBehaviour
{
    public MazeGenerator mg;
    public PlayerInputMovement pm;
    Door currentDoor;

    private void Start()
    {
        mg = FindObjectOfType<MazeGenerator>();
    }

    public void OpenDoor()
    {
        if (currentDoor)
        {
            int nextRoom;
            if (pm.CurrentCell.CellNumber == currentDoor.DoorToCellNo1)
                nextRoom = currentDoor.DoorToCellNo2;
            else
                nextRoom = currentDoor.DoorToCellNo1;

            mg.ActivateRoom(nextRoom);
            Destroy(currentDoor.gameObject);
            currentDoor = null;
        }
    }

    public void UpdateCurrentDoor(Door door)
    {
        currentDoor = door;
    }
}
