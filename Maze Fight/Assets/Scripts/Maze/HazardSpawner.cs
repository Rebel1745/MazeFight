using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public MazeGenerator mg;

    public GameObject[] RotationRoomPrefabs;
    public GameObject[] LauncherRoomPrefabs;
    public GameObject[] PokerRoomPrefabs;

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
        CreateLauncherCell(currentCell, currentFloor, hazardHolder);  // used to create projectile launchers
        /*}
    }
}*/
    }

    void CreateRotationHazardCell(MazeCell cell, GameObject floor, GameObject parent)
    {
        if (RotationRoomPrefabs.Length <= 0)
            return;

        Quaternion initialRotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

        // first pick a random hazard GO from the array
        GameObject layout = RotationRoomPrefabs[Random.Range(0, RotationRoomPrefabs.Length)];
        GameObject tempHazard;
        tempHazard = Instantiate(layout, floor.transform.position, initialRotation);
        tempHazard.transform.parent = parent.transform;
        tempHazard.name = "Rotating Arm (" + cell.CellNumber + ")";
    }

    void CreateLauncherCell(MazeCell cell, GameObject floor, GameObject parent)
    {
        if (LauncherRoomPrefabs.Length <= 0)
            return;

        float[] NSOptions = new float[] { 0f, 180f };
        float[] EWOptions = new float[] { 90f, 270f };

        Quaternion initialRotation = Quaternion.Euler(0f, 0f, 0f);
        // if cell is part of northsouth room, rotation should be y=0 or 180
        // if cell is part of eastwest room, rotation should be y=90 or 270
        if (cell.NorthSouthRoom)
            initialRotation = Quaternion.Euler(0f, NSOptions[Random.Range(0,2)], 0f);
        else if (cell.EastWestRoom)
            initialRotation = Quaternion.Euler(0f, EWOptions[Random.Range(0, 2)], 0f);

        // first pick a random hazard GO from the array
        GameObject layout = LauncherRoomPrefabs[Random.Range(0, LauncherRoomPrefabs.Length)];
        GameObject tempHazard;
        tempHazard = Instantiate(layout, floor.transform.position, initialRotation);
        tempHazard.transform.parent = parent.transform;
        tempHazard.name = "Launcher (" + cell.CellNumber + ")";
    }

    void CreatePokerHazardCell(MazeCell cell, GameObject floor, GameObject parent)
    {
        if (PokerRoomPrefabs.Length <= 0)
            return;

        // first pick a random hazard GO from the array
        GameObject layout = PokerRoomPrefabs[Random.Range(0, PokerRoomPrefabs.Length)];
        GameObject tempHazard;
        tempHazard = Instantiate(layout, floor.transform.position, Quaternion.identity);
        tempHazard.transform.parent = parent.transform;
        tempHazard.name = "Poker (" + cell.CellNumber + ")";
    }
}
