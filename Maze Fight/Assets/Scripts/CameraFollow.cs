using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform FollowTarget;
    public float SmoothSpeed = 0.01f;
    public Vector3 Offset;
    PlayerInputMovement pm;
    public MazeGenerator mg;

    // TODO:
    // Make the movement a little cleaner especially when rotating the character
    
    void LateUpdate()
    {
        // dont do anything if there is no target
        if (FollowTarget)
        {
            if (!pm)
            {
                pm = FollowTarget.GetComponentInParent<PlayerInputMovement>();
            }

            Vector3 desiredPosition = FollowTarget.position + Offset;

            if (pm.CurrentCell.EastWestRoom)
            {
                desiredPosition = new Vector3(desiredPosition.x, desiredPosition.y, pm.CurrentFloor.position.z);
            }
            else if (pm.CurrentCell.NorthSouthRoom)
            {
                desiredPosition = new Vector3(pm.CurrentFloor.position.x, desiredPosition.y, desiredPosition.z);
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
