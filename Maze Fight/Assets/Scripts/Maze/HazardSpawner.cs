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
        MazeCell currentCell;
        GameObject currentFloor;
        Transform currentHazardTransform;

        for (int y = 0; y < mg.MazeY; y++)
        {
            for (int x = 0; x < mg.MazeX; x++)
            {
                currentCell = mg.MazeCells[x, y];

                if (currentCell.roomNo != 0 && currentCell.NorthSouthRoom)
                {
                    currentFloor = currentCell.Floor;
                    currentHazardTransform = currentCell.Hazards;
                    ChooseHazardForCell(currentCell, currentFloor, currentHazardTransform);
                }
            }
        }
    }

    void ChooseHazardForCell(MazeCell mc, GameObject floor, Transform parent)
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                CreateRotationHazardCell(mc, floor, parent);
                break;
            case 2:
                CreatePokerHazardCell(mc, floor, parent);
                break;
            case 3:
                CreateLauncherCell(mc, floor, parent);
                break;
        }
    }

    void CreateRotationHazardCell(MazeCell cell, GameObject floor, Transform parent)
    {
        if (RotationRoomPrefabs.Length <= 0)
            return;

        Quaternion initialRotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

        // first pick a random hazard GO from the array
        GameObject layout = RotationRoomPrefabs[Random.Range(0, RotationRoomPrefabs.Length)];
        GameObject tempHazard;
        tempHazard = Instantiate(layout, floor.transform.position, initialRotation);
        tempHazard.transform.parent = parent;
        tempHazard.name = "Rotating Arm (" + cell.CellNumber + ")";
    }

    void CreateLauncherCell(MazeCell cell, GameObject floor, Transform parent)
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
        tempHazard.transform.parent = parent;
        tempHazard.name = "Launcher (" + cell.CellNumber + ")";
    }

    void CreatePokerHazardCell(MazeCell cell, GameObject floor, Transform parent)
    {
        if (PokerRoomPrefabs.Length <= 0)
            return;

        Quaternion initialRotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

        // first pick a random hazard GO from the array
        GameObject layout = PokerRoomPrefabs[Random.Range(0, PokerRoomPrefabs.Length)];
        GameObject tempHazard;
        tempHazard = Instantiate(layout, floor.transform.position, initialRotation);
        tempHazard.transform.parent = parent;
        tempHazard.name = "Poker (" + cell.CellNumber + ")";
    }
}
