using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner_old : MonoBehaviour
{
    public MazeGenerator mg;

    public GameObject Poker;
    public GameObject RotatingArm;
    public GameObject ProjectileLauncher;

    public void CreateHazards()
    {
        GameObject hazardHolder = new GameObject
        {
            name = "Hazards"
        };
        // test cell will be the second cell created
        MazeCell currentCell;
        GameObject currentFloor;

        // used for debugging
        currentCell = mg.MazeCells[1, 0];
        currentFloor = currentCell.Floor;

        /*for (int y = 0; y < mg.MazeY; y++)
        {
            for (int x = 0; x < mg.MazeX; x++)
            {
                if (x != 0 || y != 0)
                {
                    currentCell = mg.MazeCells[x, y];
                    currentFloor = currentCell.Floor;*/

                    //CreateRotationHazardCell(currentCell, currentFloor, hazardHolder);  // Used to create rotating arm traps
                    //CreatePokerHazardCell(currentCell, currentFloor, hazardHolder);  // Used to create poker traps
                    CreateProjectileLauncherCell(currentCell, currentFloor, hazardHolder);
                /*}
            }
        }*/

    }

    void CreateProjectileLauncherCell(MazeCell currentCell, GameObject currentFloor, GameObject hazardHolder)
    {
        GameObject tempHazard;
        float launcherHeight = ProjectileLauncher.transform.localScale.y;
        tempHazard = Instantiate(ProjectileLauncher, currentFloor.transform.position, Quaternion.identity);
        tempHazard.transform.position = new Vector3(tempHazard.transform.position.x, tempHazard.transform.position.y + launcherHeight, tempHazard.transform.position.z);
        tempHazard.transform.parent = hazardHolder.transform;
        tempHazard.name = "Projectile Launcher (" + currentCell.CellNumber + ")";
    }

    void CreateRotationHazardCell(MazeCell currentCell, GameObject currentFloor, GameObject hazardHolder)
    {
        GameObject tempHazard;
        tempHazard = Instantiate(RotatingArm, currentFloor.transform.position, Quaternion.identity);
        tempHazard.transform.parent = hazardHolder.transform;
        tempHazard.name = "Rotating Arm (" + currentCell.CellNumber + ")";
    }

    void CreatePokerHazardCell(MazeCell curentCell, GameObject currentFloor, GameObject hazardHolder)
    {
        GameObject tempHazard;
        float pokerWidth = Poker.transform.localScale.x;
        float pokerLength = Poker.transform.localScale.z;
        float xOffset, zOffset;

        int totalXHazards = Mathf.CeilToInt(mg.floorLength / pokerWidth);
        int totalZHazards = Mathf.CeilToInt(mg.floorLength / pokerLength);

        float startPosX = currentFloor.transform.position.x - (mg.floorLength / 2) + (pokerWidth / 2);
        float startPosZ = currentFloor.transform.position.z - (mg.floorLength / 2) + (pokerLength / 2);

        for (int zHaz = 0; zHaz < totalZHazards; zHaz++)
        {
            for (int xHaz = 0; xHaz < totalXHazards; xHaz++)
            {
                xOffset = xHaz * pokerLength;
                zOffset = zHaz * pokerWidth;
                tempHazard = Instantiate(Poker, new Vector3(startPosX + xOffset, 0f, startPosZ + zOffset), Quaternion.identity);
                tempHazard.transform.parent = hazardHolder.transform;
                tempHazard.name = "Poker (" + xHaz + ", " + zHaz + ")";
            }
        }
    }
}
