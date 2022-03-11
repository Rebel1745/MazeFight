using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform FollowTarget;
    public float SmoothSpeed = 0.01f;
    public float SnapThreshold = 0.1f;
    public Vector3 Offset;
    public PlayerInputMovement pm;
    public MazeGenerator mg;
    
    void LateUpdate()
    {
        if (FollowTarget)
        {
            Vector3 desiredPosition = FollowTarget.position + Offset;

            if (pm.CameraSnap)
            {
                transform.position = desiredPosition;
            }
            else
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
                transform.position = smoothedPosition;
                if (Vector3.Distance(transform.position, desiredPosition) <= SnapThreshold)
                    pm.CameraSnap = true;
            }
        }
    }
}
