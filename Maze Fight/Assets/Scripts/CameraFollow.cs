using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform FollowTarget;
    public float SmoothSpeedWalk = 0.2f;
    public float SmoothSpeedRoll = 0.1f;
    public float SnapThreshold = 0.1f;
    public Vector3 Offset;
    public PlayerInputMovement pm;
    public MazeGenerator mg;
    private float currentSmoothSpeed;
    
    void LateUpdate()
    {
        if (FollowTarget)
        {
            Vector3 desiredPosition = FollowTarget.position + Offset;
            
            currentSmoothSpeed = pm.isBodyStandard ? SmoothSpeedWalk : SmoothSpeedRoll;

            Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, currentSmoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
