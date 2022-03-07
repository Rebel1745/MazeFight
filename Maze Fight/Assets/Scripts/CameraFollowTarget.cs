using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public GameObject pl;
    public PlayerInputMovement pm;

    public LayerMask WhatIsWall;
    public float MaxLookaheadDistance = 2f;
    public float WallDistanceThreshold = 1f;
    public float distToWall;
    private Vector3 newPos;

    void Update()
    {
        CalculateLocation();
        UpdateLocation();
    }

    void CalculateLocation()
    {
        if (pm)
        {
            float xInput = pm.moveInput.x;
            float zInput = pm.moveInput.y;
            //float distToWall;
            float lookaheadDistance = 0f;

            if (pm.CurrentCell.SingleCellRoom)
            {
                // if we are in a single cell room, set the camera position to the centre of the room (floor)
                newPos = pm.CurrentCell.Floor.transform.position;
            }
            else if (pm.CurrentCell.EastWestRoom)
            {
                if(xInput == 0)
                {
                    lookaheadDistance = 0;
                }
                else if(xInput > 0)
                {
                    if(Physics.Raycast(pl.transform.position, Vector3.right, out RaycastHit hit, Mathf.Infinity, WhatIsWall)){
                        distToWall = Vector3.Distance(transform.position, hit.transform.position);
                        if (distToWall < WallDistanceThreshold)
                            distToWall = 0;
                        lookaheadDistance = Mathf.Min(distToWall, MaxLookaheadDistance);
                    }
                }
                else if(xInput < 0)
                {
                    if (Physics.Raycast(pl.transform.position, Vector3.left, out RaycastHit hit, Mathf.Infinity, WhatIsWall))
                    {
                        distToWall = Vector3.Distance(transform.position, hit.transform.position);
                        if (distToWall < WallDistanceThreshold)
                            distToWall = 0;
                        lookaheadDistance = -Mathf.Min(distToWall, MaxLookaheadDistance);
                    }
                }
                // if we are rolling, don't lookahead
                if (!pm.isBodyStandard)
                    lookaheadDistance = 0;
                // if we are in an east west room, limit the z position
                newPos = new Vector3(pl.transform.position.x + lookaheadDistance, 0.5f, pm.CurrentCell.Floor.transform.position.z);
            }
            else if(pm.CurrentCell.NorthSouthRoom)
            {
                if (zInput == 0)
                {
                    lookaheadDistance = 0;
                }
                else if (zInput > 0)
                {
                    if (Physics.Raycast(pl.transform.position, Vector3.forward, out RaycastHit hit, Mathf.Infinity, WhatIsWall))
                    {
                        distToWall = Vector3.Distance(transform.position, hit.transform.position);
                        if (distToWall < WallDistanceThreshold)
                            distToWall = 0;
                        lookaheadDistance = Mathf.Min(distToWall, MaxLookaheadDistance);
                    }
                }
                else if (zInput < 0)
                {
                    if (Physics.Raycast(pl.transform.position, Vector3.back, out RaycastHit hit, Mathf.Infinity, WhatIsWall))
                    {
                        distToWall = Vector3.Distance(transform.position, hit.transform.position);
                        if (distToWall < WallDistanceThreshold)
                            distToWall = 0;
                        lookaheadDistance = -Mathf.Min(distToWall, MaxLookaheadDistance);
                    }
                }
                // if we are rolling, don't lookahead
                if (!pm.isBodyStandard)
                    lookaheadDistance = 0;
                // north south room, limit the x position
                newPos = new Vector3(pm.CurrentCell.Floor.transform.position.x, 0.5f, pl.transform.position.z + lookaheadDistance);
            }            
        }        
    }

    void UpdateLocation()
    {
        transform.position = newPos;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.right * 10, Color.blue);
        Debug.DrawRay(transform.position, Vector3.left * 10, Color.red);
        Debug.DrawRay(transform.position, Vector3.forward * 10, Color.green);
        Debug.DrawRay(transform.position, Vector3.back * 10, Color.black);
    }
}
