using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public GameObject pl;
    public PlayerInputMovement pm;

    private Vector3 newPos;
    public float CameraMoveSpeed = 0.1f;

    void Update()
    {
        CalculateLocation();
        UpdateLocation();
    }

    void CalculateLocation()
    {
        if (pm)
        {
            float xInput = pm.MoveInput.x;
            float zInput = pm.MoveInput.y;

            if (pm.CurrentCell.SingleCellRoom)
            {
                // if we are in a single cell room, set the camera position to the centre of the room (floor)
                newPos = pm.CurrentCell.Floor.transform.position;
            }
            else if (pm.CurrentCell.EastWestRoom)
            {
                // if we are in an east west room, limit the z position
                newPos = new Vector3(pl.transform.position.x, 0.5f, pm.CurrentCell.Floor.transform.position.z);
            }
            else if(pm.CurrentCell.NorthSouthRoom)
            {
                // north south room, limit the x position
                newPos = new Vector3(pm.CurrentCell.Floor.transform.position.x, 0.5f, pl.transform.position.z);
            }            
        }        
    }

    void UpdateLocation()
    {
        transform.position = Vector3.Lerp(transform.position, newPos, CameraMoveSpeed);
    }
}
