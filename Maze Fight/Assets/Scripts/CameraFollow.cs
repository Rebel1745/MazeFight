using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform FollowTarget;
    public float SmoothSpeed = 0.01f;
    public Vector3 Offset;
    public PlayerInputMovement pm;
    public MazeGenerator mg;
    
    void LateUpdate()
    {
        if (FollowTarget)
        {
            Vector3 desiredPosition = FollowTarget.position + Offset;

            if (pm.isBodyStandard)
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
                transform.position = smoothedPosition;
            }
            else
            {
                transform.position = desiredPosition;
            }
        }
    }
}
