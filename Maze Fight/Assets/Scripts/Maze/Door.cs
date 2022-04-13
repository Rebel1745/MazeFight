using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int DoorToCellNo1 = -1;
    public int DoorToCellNo2 = -1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponentInParent<PlayerDoorController>().UpdateCurrentDoor(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInParent<PlayerDoorController>().UpdateCurrentDoor(null);
        }
    }
}
