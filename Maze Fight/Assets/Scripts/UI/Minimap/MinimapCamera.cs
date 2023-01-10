using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform FollowTarget;

    private float camY;
    private Vector3 newPos;

    void Start()
    {
        camY = transform.position.y;
    }

    void LateUpdate()
    {
        if (FollowTarget)
        {
            newPos = FollowTarget.position;
            newPos.y = camY;
            transform.position = newPos;
        }        
    }
}
