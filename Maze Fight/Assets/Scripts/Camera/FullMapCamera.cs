using UnityEngine;

public class FullMapCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomIncrement = 0.1f;
    bool isStarted = false;
    bool isFinished = false;

    Vector3 startingRoomPos, endingRoomPos;

    float maximumZoom = 0;

    public void StartCongigureZoom(Vector3 start, Vector3 end)
    {

        if (!isStarted)
        {
            startingRoomPos = start;
            endingRoomPos = end;
            CentreCamera();
            isStarted = true;

            GameObject sp = new GameObject();
            sp.name = "Start pos";
            sp.transform.position = start;
            GameObject ep = new GameObject();
            ep.name = "End pos";
            ep.transform.position = end;
        }
    }

    void CentreCamera()
    {
        var bounds = new Bounds(startingRoomPos, Vector3.zero);
        bounds.Encapsulate(endingRoomPos);

        cam.transform.position = new Vector3(bounds.center.x, cam.transform.position.y, bounds.center.z);
    }

    void ConfigureZoom()
    {
        bool xVis = false, yVis = false;

        // check if the starting room is visible
        Vector3 viewPos = cam.WorldToViewportPoint(startingRoomPos);
        if (viewPos.x <= 1 && viewPos.x >= 0 && viewPos.y <= 1 && viewPos.y >= 0)
            xVis = true;

        // check if the ending room is visible
        viewPos = cam.WorldToViewportPoint(endingRoomPos);
        if (viewPos.x <= 1 && viewPos.x >= 0 && viewPos.y <= 1 && viewPos.y >= 0)
            yVis = true;

        if (xVis && yVis)
        {
            isFinished = true;
            maximumZoom = cam.orthographicSize;
        }
        else
        {
            // zoom out
            cam.orthographicSize += zoomIncrement;
        }        
    }

    private void Update()
    {
        if (isStarted && !isFinished)
            ConfigureZoom();
    }
}
