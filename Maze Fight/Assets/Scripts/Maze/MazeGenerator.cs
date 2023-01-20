using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public CharacterSpawner cs;
    public HazardSpawner hs;
    public FullMapCamera fmc;

    // maze setup
    public MazeCell[,] MazeCells;
    public GameObject Wall;
    public GameObject Floor;
    public GameObject UnderFloor;
    public GameObject Door;
    public GameObject StartingRoom;
    public float CellsInStartingRoom = 3;
    public GameObject EndingRoom;
    public float CellsInEndingRoom = 3;
    private float wallLength = 0.0f;
    private float wallHeight = 0.0f;
    private float wallWidth = 0.0f;
    public float floorLength = 0.0f;
    private float doorLength = 0.0f;
    private float doorHeight = 0.0f;
    public float UnderFloorHeight = -0.5f;
    public float UnderFloorColliderOffsetY = 0.15f;
    public int MazeX = 5;
    public int MazeY = 5;

    // maze creation
    private int currentX = 0;
    private int currentY = 0;
    private bool courseComplete = false;
    private int currentRoom = 1;
    public int totalRooms = 0;
    private bool roomsComplete = false;

    // Health bars
    public GameObject PlayerStatusBar;
    public GameObject EnemyHealthBar;

    // Maps
    public GameObject FullMap;
    public GameObject MiniMap;

    //TODO: 
    // write a DrawMaze() funtion rather than creating walls first, then destroying them, then creating doors

    public void GenerateMaze()
    {
        SaveUIReferences();
        InitialiseMaze();
        CreateMaze();
        CreateRooms();
        CreateDoors();
        UpdateAdjoiningWalls();
        cs.CreateTrainingDummy();
        cs.CreatePlayer();
        //cs.CreateEnemies();
        //hs.CreateHazards();
        //DeactivateRooms();
        CreateStartEndRooms();
    }

    // TODO: give the floors in the extra rooms numbers and create a MazeCell for each and add it to the array, with meta data
    void CreateStartEndRooms()
    {
        // Starting Room
        float startPosX = -CellsInStartingRoom * floorLength;
        GameObject sRoom = Instantiate(StartingRoom, new Vector3(startPosX, 0f, 0f), Quaternion.identity);
        Vector3 startingRoomPosition = new Vector3(startPosX - floorLength, 0f, 0f);

        // remove the west wall from the first cell in the maze to replace it with the starting room door
        MazeCell startCell = MazeCells[0, 0];
        DestroyWall(startCell, startCell.WestWall, 4);

        // loop through the cells in the prefabs to create a MazeCell and number the floor
        int currentX = 0, currentY = MazeY, currentCellNo;
        Floor[] floors = sRoom.GetComponentsInChildren<Floor>();
        foreach(Floor f in floors)
        {
            currentCellNo = currentY * MazeX + currentX;
            f.FloorCellNo = currentCellNo;
            MazeCells[currentX, currentY] = new MazeCell
            {
                CellNumber = currentCellNo,
                CellX = currentX,
                CellY = currentY,
                EastWestRoom = true,
                NorthSouthRoom = false,
                SingleCellRoom = false,
                Floor = f.gameObject,
                CanBeActivated = false
            };
            currentX++;
        }

        // set the cell numbers on the door script so it can be opened
        Door sDoor = sRoom.GetComponentInChildren<Door>();
        sDoor.DoorToCellNo1 = 0;
        sDoor.DoorToCellNo2 = currentY * MazeX + currentX - 1;

        // Ending Room
        startPosX = floorLength * MazeX;
        float endPosX = startPosX + CellsInEndingRoom * floorLength;
        float startPosZ = floorLength * (MazeY - 1);
        GameObject eRoom = Instantiate(EndingRoom, new Vector3(startPosX, 0f, startPosZ), Quaternion.identity);
        Vector3 endingRoomPosition = new Vector3(endPosX, 0f, floorLength * MazeY);

        // remove the east wall from the last cell in the maze to replace it with the ending room door
        MazeCell endCell = MazeCells[MazeX - 1, MazeY - 1];
        DestroyWall(endCell, endCell.EastWall, 4);

        // loop through the cells in the prefabs to create a MazeCell and number the floor
        floors = eRoom.GetComponentsInChildren<Floor>();
        foreach (Floor f in floors)
        {
            currentCellNo = currentY * MazeX + currentX;
            f.FloorCellNo = currentCellNo;
            MazeCells[currentX, currentY] = new MazeCell
            {
                CellNumber = currentCellNo,
                CellX = currentX,
                CellY = currentY,
                EastWestRoom = true,
                NorthSouthRoom = false,
                SingleCellRoom = false,
                Floor = f.gameObject,
                CanBeActivated = false
            };
            currentX++;
        }

        // set the cell numbers on the door script so it can be opened
        Door eDoor = eRoom.GetComponentInChildren<Door>();
        eDoor.DoorToCellNo1 = currentY * MazeX + currentX - 1;
        eDoor.DoorToCellNo2 = (MazeX * MazeY) - 1;

        // setup the zoom for the full map camera to make full map visible
        fmc.StartCongigureZoom(startingRoomPosition, endingRoomPosition);
    }

    void SaveUIReferences()
    {
        PlayerStatusBar = GameObject.FindGameObjectWithTag("PlayerStatusBar");
        EnemyHealthBar = GameObject.FindGameObjectWithTag("EnemyHealthBar");
        FullMap = GameObject.FindGameObjectWithTag("FullMap");
        MiniMap = GameObject.FindGameObjectWithTag("MiniMap");
    }

    void DeactivateRooms()
    {
        MazeCell currentCell;
        // if we arent in the first room, deactivate all walls and doors
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                currentCell = MazeCells[x, y];

                if (currentCell.CanBeActivated)
                {
                    // Deactivate walls separately so shared walls can be activated from adjacent rooms
                    if (currentCell.HasNorthWall)
                        currentCell.NorthWall.SetActive(false);
                    if (currentCell.HasSouthWall)
                        currentCell.SouthWall.SetActive(false);
                    if (currentCell.HasEastWall)
                        currentCell.EastWall.SetActive(false);
                    if (currentCell.HasWestWall)
                        currentCell.WestWall.SetActive(false);
                    currentCell.Floor.SetActive(false);
                    
                    //currentCell.WallsAndFloor.gameObject.SetActive(false);
                    currentCell.Hazards.gameObject.SetActive(false);
                    currentCell.Enemies.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ActivateRoom(int cellNo)
    {

        MazeCell currentCell = GetMazeCellFromInt(cellNo);
        int roomNo = currentCell.roomNo;

        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                currentCell = MazeCells[x, y];

                if (currentCell.roomNo == roomNo)
                {
                    if (currentCell.NorthWall)
                        currentCell.NorthWall.SetActive(true);
                    if (currentCell.SouthWall)
                        currentCell.SouthWall.SetActive(true);
                    if (currentCell.EastWall)
                        currentCell.EastWall.SetActive(true);
                    if (currentCell.WestWall)
                        currentCell.WestWall.SetActive(true);
                    currentCell.Floor.SetActive(true);

                    //currentCell.WallsAndFloor.gameObject.SetActive(true);
                    currentCell.Hazards.gameObject.SetActive(true);
                    currentCell.Enemies.gameObject.SetActive(true);
                }
            }
        }
    }
    
    void CreateDoors()
    {
        GameObject tempDoor;

        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                if (x + 1 < MazeX && MazeCells[x, y].roomNo != MazeCells[x + 1, y].roomNo && !MazeCells[x, y].HasEastWall)
                {
                    tempDoor = Instantiate(Door, new Vector3((x * doorLength) + (doorLength / 2f), doorHeight / 2f, y * doorLength), Quaternion.identity) as GameObject;
                    MazeCells[x, y].EastWall = tempDoor;
                    MazeCells[x, y].EastWall.name = "East Door " + x + "," + y;
                    MazeCells[x, y].HasEastWall = true;
                    MazeCells[x, y].EastWall.transform.Rotate(Vector3.up * 90f);
                    tempDoor.transform.parent = MazeCells[x, y].WallsAndFloor;
                    int eastDoorToCellNo = y * MazeX + x + 1;
                    int westDoorToCellNo = y * MazeX + x;
                    tempDoor.GetComponentInChildren<Door>().DoorToCellNo1 = eastDoorToCellNo;
                    tempDoor.GetComponentInChildren<Door>().DoorToCellNo2 = westDoorToCellNo;
                }
                if (y + 1 < MazeY && MazeCells[x, y].roomNo != MazeCells[x, y + 1].roomNo && !MazeCells[x, y].HasNorthWall)
                {
                    tempDoor = Instantiate(Door, new Vector3(x * doorLength, doorHeight / 2f, (y * doorLength) + (doorLength / 2f)), Quaternion.identity) as GameObject;
                    MazeCells[x, y].NorthWall = tempDoor;
                    MazeCells[x, y].NorthWall.name = "North Door " + x + "," + y;
                    MazeCells[x, y].HasNorthWall = true;
                    tempDoor.transform.parent = MazeCells[x, y].WallsAndFloor;
                    int northDoorToCellNo = (y + 1) * MazeX + x;
                    int southDoorToCellNo = y * MazeX + x;
                    tempDoor.GetComponentInChildren<Door>().DoorToCellNo1 = northDoorToCellNo;
                    tempDoor.GetComponentInChildren<Door>().DoorToCellNo2 = southDoorToCellNo;
                }
                else if (MazeCells[x, y].SingleCellRoom)
                {
                    // cell is a SingleCellRoom, the door has already been created above
                }
            }
        }
    }

    void CreateRooms()
    {
        // reset if maze cells have been visited
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                MazeCells[x, y].visited = false;
            }
        }

        // start at (0, 0)
        MazeCell currentCell = MazeCells[0, 0];
        
        while (!roomsComplete)
        {
            VisitCellAndCheckConnection(currentCell, false, true);
            currentCell = GetNextUnvisitedCell();
            currentRoom++;
        }
    }

    void ShowDebugCells()
    {
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                Debug.Log( MazeCells[x, y].CellNumber + " (" + MazeCells[x, y].CellX.ToString() + ", " + MazeCells[x, y].CellY.ToString() + ")");
            }
        }
    }

    void VisitCellAndCheckConnection(MazeCell cell, bool dir, bool isFirstCellInRoom)
    {
        if (cell.visited)
        {
            Debug.LogError("Visited cell getting visited again!");
            return;
        }
           

        MazeCell nextCell = null;
        // mark cell as visited so we dont come back

        // dir: false = east, true = north
        if (dir)
        {
            // north
            if(!cell.HasNorthWall)
            {
                cell.NorthSouthRoom = true;
                cell.visited = true;
                cell.roomNo = currentRoom;
                nextCell = MazeCells[cell.CellX, cell.CellY + 1];
                VisitCellAndCheckConnection(nextCell, dir, false);
            }
            else
            {
                cell.visited = true;
                cell.roomNo = currentRoom;
                // wall here
                // if this is the first cell in room then it is a one cell room
                if (isFirstCellInRoom)
                    cell.SingleCellRoom = true;
                else
                    cell.NorthSouthRoom = true;              
            }
        }
        else
        {
            // east
            if(!cell.HasEastWall)
            {
                cell.visited = true;
                cell.roomNo = currentRoom;
                nextCell = MazeCells[cell.CellX + 1, cell.CellY];
                // as we are going to the right, we may run into a NorthWestRoom
                if (!nextCell.visited)
                {
                    cell.EastWestRoom = true;
                    VisitCellAndCheckConnection(nextCell, dir, false);
                }                    
                else
                {
                    if (isFirstCellInRoom)
                        cell.SingleCellRoom = true;
                    else
                        cell.EastWestRoom = true;
                }
            }
            else
            {
                // there is a wall here, if this is the first cell in room, run again but looking north
                if (isFirstCellInRoom)
                {
                    VisitCellAndCheckConnection(cell, !dir, isFirstCellInRoom);
                }
                else
                {
                    cell.visited = true;
                    cell.roomNo = currentRoom;
                    cell.EastWestRoom = true;
                }                
            }
        }

    }

    MazeCell GetNextUnvisitedCell()
    {
        MazeCell nextCell = null;

        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                if (MazeCells[x, y].visited == false)
                    return MazeCells[x, y];
            }
        }

        if (nextCell == null)
        {
            roomsComplete = true;
            totalRooms = currentRoom;
        }

        return nextCell;
    }

    void CreateMaze()
    {
        MazeCells[currentX, currentY].visited = true;

        while (!courseComplete)
        {
            Kill(); // Will run until it hits a dead end.
            Hunt(); // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
        }
    }

    private void Kill()
    {
        while (RouteStillAvailable(currentX, currentY))
        {
            int direction = Random.Range (1, 5);

            if (direction == 1 && CellIsAvailable(currentX, currentY + 1))
            {
                // North
                if(MazeCells[currentX, currentY].NorthWall != null)
                    DestroyWall(MazeCells[currentX, currentY], MazeCells[currentX, currentY].NorthWall, 1);
                if(MazeCells[currentX, currentY + 1].SouthWall != null)
                    DestroyWall(MazeCells[currentX, currentY + 1], MazeCells[currentX, currentY + 1].SouthWall, 2);
                currentY++;
            }
            else if (direction == 2 && CellIsAvailable(currentX, currentY - 1))
            {
                // South
                if(MazeCells[currentX, currentY].SouthWall != null)
                    DestroyWall(MazeCells[currentX, currentY], MazeCells[currentX, currentY].SouthWall, 2);
                if (MazeCells[currentX, currentY - 1].NorthWall != null)
                    DestroyWall(MazeCells[currentX, currentY - 1], MazeCells[currentX, currentY - 1].NorthWall, 1);
                currentY--;
            }
            else if (direction == 3 && CellIsAvailable(currentX + 1, currentY))
            {
                // east
                if(MazeCells[currentX, currentY].EastWall != null)
                    DestroyWall(MazeCells[currentX, currentY], MazeCells[currentX, currentY].EastWall, 3);
                if (MazeCells[currentX + 1, currentY].WestWall != null)
                    DestroyWall(MazeCells[currentX + 1, currentY], MazeCells[currentX + 1, currentY].WestWall, 4);
                currentX++;
            }
            else if (direction == 4 && CellIsAvailable(currentX - 1, currentY))
            {
                // west
                if(MazeCells[currentX, currentY].WestWall != null)
                    DestroyWall(MazeCells[currentX, currentY], MazeCells[currentX, currentY].WestWall, 4);
                if (MazeCells[currentX - 1, currentY].EastWall != null)
                    DestroyWall(MazeCells[currentX - 1, currentY], MazeCells[currentX - 1, currentY].EastWall, 3);
                currentX--;
            }

            MazeCells[currentX, currentY].visited = true;
        }
    }

    private void Hunt()
    {
        courseComplete = true; // Set it to this, and see if we can prove otherwise below!
        
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                if (!MazeCells[x, y].visited && CellHasAnAdjacentVisitedCell(x, y))
                {
                    courseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    currentX = x;
                    currentY = y;
                    DestroyAdjacentWall(currentX, currentY);
                    MazeCells[currentX, currentY].visited = true;
                    return; // Exit the function
                }
            }
        }
    }


    private bool RouteStillAvailable(int x, int y)
    {
        int availableRoutes = 0;

        // can we go west?
        if (x > 0 && !MazeCells[x - 1, y].visited)
        {
            availableRoutes++;
        }

        // can we go east?
        if (x < MazeX - 1 && !MazeCells[x + 1, y].visited)
        {
            availableRoutes++;
        }

        // can we go south?
        if (y > 0 && !MazeCells[x, y - 1].visited)
        {
            availableRoutes++;
        }

        // can we go north?
        if (y < MazeY - 1 && !MazeCells[x, y + 1].visited)
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private bool CellIsAvailable(int x, int y)
    {
        if (x >= 0 && x < MazeX && y >= 0 && y < MazeY && !MazeCells[x, y].visited)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DestroyWall(MazeCell cell, GameObject wall, int orientation)
    {
        GameObject.Destroy(wall);

        switch (orientation)
        {
            case 1:
                cell.HasNorthWall = false;
                cell.NorthWall = null;
                break;
            case 2:
                cell.HasSouthWall = false;
                cell.SouthWall = null;
                break;
            case 3:
                cell.HasEastWall = false;
                cell.EastWall = null;
                break;
            case 4:
                cell.HasWestWall = false;
                cell.WestWall = null;
                break;
        }
    }

    private bool CellHasAnAdjacentVisitedCell(int x, int y)
    {
        int visitedCells = 0;

        // Look 1 row up (north)
        if (y < (MazeY - 2) && MazeCells[x, y + 1].visited)
        {
            visitedCells++;
        }

        // Look one row down (south)
        if (y < 0 && MazeCells[x, y - 1].visited)
        {
            visitedCells++;
        }

        // Look one row left (west)
        if (x > 0 && MazeCells[x - 1, y].visited)
        {
            visitedCells++;
        }

        // Look one row right (east)
        if (x < (MazeX - 2) && MazeCells[x + 1, y].visited)
        {
            visitedCells++;
        }

        // return true if there are any adjacent visited cells to this one
        return visitedCells > 0;
    }

    private void DestroyAdjacentWall(int x, int y)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direction = Random.Range (1, 5);

            // north
            if (direction == 1 && y < (MazeY - 2) && MazeCells[x, y + 1].visited)
            {
                if(MazeCells[x, y].NorthWall != null)
                    DestroyWall(MazeCells[x, y], MazeCells[x, y].NorthWall, 1);
                if(MazeCells[x, y + 1].SouthWall != null)
                    DestroyWall(MazeCells[x, y + 1], MazeCells[x, y + 1].SouthWall, 2);
                wallDestroyed = true;
            }
            // south
            else if (direction == 2 && y > 0 && MazeCells[x, y - 1].visited)
            {
                if(MazeCells[x, y].SouthWall != null)
                    DestroyWall(MazeCells[x, y], MazeCells[x, y].SouthWall, 2);
                if(MazeCells[x, y - 1].NorthWall != null)
                    DestroyWall(MazeCells[x, y - 1], MazeCells[x, y - 1].NorthWall, 1);
                wallDestroyed = true;
            }
            // east
            else if (direction == 3 && x < (MazeX - 2) && MazeCells[x + 1, y].visited)
            {
                if (MazeCells[x, y].EastWall != null)
                    DestroyWall(MazeCells[x, y], MazeCells[x, y].EastWall, 3);
                if (MazeCells[x + 1, y].WestWall != null)
                    DestroyWall(MazeCells[x + 1, y], MazeCells[x + 1, y].WestWall, 4);
                wallDestroyed = true;
            }
            // west
            else if (direction == 4 && x > 0 && MazeCells[x - 1, y].visited)
            {
                if (MazeCells[x, y].WestWall != null)
                    DestroyWall(MazeCells[x, y], MazeCells[x, y].WestWall, 4);
                if (MazeCells[x - 1, y].EastWall != null)
                    DestroyWall(MazeCells[x - 1, y], MazeCells[x - 1, y].EastWall, 3);
                wallDestroyed = true;
            }
        }

    }

    // sets up all cells with walls on all sides
    void InitialiseMaze()
    {
        wallLength = Wall.transform.localScale.x;
        wallHeight = Wall.transform.localScale.y;
        wallWidth = Wall.transform.localScale.z;
        floorLength = Floor.transform.localScale.x;
        doorLength = Door.transform.localScale.x;
        doorHeight = Door.transform.localScale.y;

        MazeCells = new MazeCell[MazeX, MazeY + 1]; // add 1 to the y length to allow the starting and ending rooms to be added

        GameObject MazeCellHolder = new GameObject
        {
            name = "Maze Cells"
        };
        MazeCellHolder.transform.parent = transform;

        GameObject tempUnderFloor, tempFloor, tempWall;

        // create the black void of the underfloor to be slightly bigger than the whole maze
        float underFloorStartX = floorLength * (MazeX / 2) - (floorLength / 2);
        float underFloorStartY = -0.45f;
        float underFloorStartZ = floorLength * (MazeY / 2) - (floorLength / 2);
        // make the length and width of the underfloor double the size of the maze so it extends to cover the whole camera
        float underfloorLength = floorLength * (MazeX * 3) + 100;
        float underfloorWidth = floorLength * (MazeY * 3) + 100;

        tempUnderFloor = Instantiate(UnderFloor, new Vector3(underFloorStartX, underFloorStartY, underFloorStartZ), Quaternion.identity);
        tempUnderFloor.transform.localScale = new Vector3(underfloorLength, 1f, underfloorWidth);
        BoxCollider bc = tempUnderFloor.AddComponent<BoxCollider>();
        bc.center.Set(0f, UnderFloorColliderOffsetY, 0f);

        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                int cellNo = y * MazeX + x;

                GameObject currentCell = new GameObject
                {
                    name = "Maze Cell " + cellNo.ToString()
                };

                GameObject wallsAndFloor = new GameObject
                {
                    name = "Walls and Floor"
                };

                GameObject hazards = new GameObject
                {
                    name = "Hazards"
                };

                GameObject enemies = new GameObject
                {
                    name = "Enemies"
                };

                MazeCells[x, y] = new MazeCell
                {
                    CellNumber = cellNo,
                    CellX = x,
                    CellY = y,
                    CellHolder = currentCell.transform,
                    WallsAndFloor = wallsAndFloor.transform,
                    Hazards = hazards.transform,
                    Enemies = enemies.transform
                };

                currentCell.transform.parent = MazeCellHolder.transform;
                wallsAndFloor.transform.parent = currentCell.transform;
                hazards.transform.parent = currentCell.transform;
                enemies.transform.parent = currentCell.transform;

                // start with the floor
                tempFloor = Instantiate(Floor, new Vector3(x * floorLength, 0, y * floorLength), Quaternion.identity) as GameObject;
                MazeCells[x, y].Floor = tempFloor;
                MazeCells[x, y].Floor.name = "Floor " + x + "," + y;
                MazeCells[x, y].Floor.GetComponent<Floor>().FloorCellNo = cellNo;
                tempFloor.transform.parent = wallsAndFloor.transform;

                if (y == 0)
                {
                    tempWall = Instantiate(Wall, new Vector3(x * wallLength, wallHeight / 2f, (y * wallLength) - (wallLength / 2f)), Quaternion.identity) as GameObject;
                    MazeCells[x, y].SouthWall = tempWall;
                    MazeCells[x, y].SouthWall.name = "South Wall " + x + "," + y;
                    MazeCells[x, y].HasSouthWall = true;
                    tempWall.transform.parent = wallsAndFloor.transform;
                }

                tempWall = Instantiate(Wall, new Vector3(x * wallLength, wallHeight / 2f, (y * wallLength) + (wallLength / 2f)), Quaternion.identity) as GameObject;
                MazeCells[x, y].NorthWall = tempWall;
                MazeCells[x, y].NorthWall.name = "North Wall " + x + "," + y;
                MazeCells[x, y].HasNorthWall = true;
                tempWall.transform.parent = wallsAndFloor.transform;

                if (x == 0)
                {
                    tempWall = Instantiate(Wall, new Vector3((x * wallLength) - (wallLength / 2f), wallHeight / 2f, y * wallLength), Quaternion.identity) as GameObject;
                    MazeCells[x, y].WestWall = tempWall;
                    MazeCells[x, y].WestWall.name = "West Wall " + x + "," + y;
                    MazeCells[x, y].HasWestWall = true;
                    MazeCells[x, y].WestWall.transform.Rotate(Vector3.up * 90f);
                    tempWall.transform.parent = wallsAndFloor.transform;
                }

                tempWall = Instantiate(Wall, new Vector3((x * wallLength) + (wallLength / 2f), wallHeight / 2f, y * wallLength), Quaternion.identity) as GameObject;
                MazeCells[x, y].EastWall = tempWall;
                MazeCells[x, y].EastWall.name = "East Wall " + x + "," + y;
                MazeCells[x, y].HasEastWall = true;
                MazeCells[x, y].EastWall.transform.Rotate(Vector3.up * 90f);
                tempWall.transform.parent = wallsAndFloor.transform;
                //MazeCell temp = GetMazeCellFromInt(cellNo);
            }
        }
    }

    void UpdateAdjoiningWalls()
    {
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                // if there is a cell above this one then its south wall IS this north wall
                if (y < MazeY - 1)
                    MazeCells[x, y + 1].SouthWall = MazeCells[x, y].NorthWall;

                // if there is a cell right of this one then its west wall IS this east wall
                if (x < MazeX - 1)
                    MazeCells[x + 1, y].WestWall = MazeCells[x, y].EastWall;
            }
        }
    }

    public MazeCell GetMazeCellFromInt(int cellNo)
    {
        int cellX = cellNo % MazeX;
        int cellY = Mathf.FloorToInt (cellNo / MazeX);
        
        return MazeCells[cellX, cellY];
        
    }
}
