using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // maze setup
    public MazeCell[,] MazeCells;
    public GameObject Wall;
    public GameObject Floor;
    public GameObject Door;
    public GameObject Player;
    private float wallLength = 0.0f;
    private float floorLength = 0.0f;
    public int MazeX = 5;
    public int MazeY = 5;
    private int mazeCellCount;
    // for debugging purposes only
    public MazeCell[] Cells;

    // maze creation
    private int currentX = 0;
    private int currentY = 0;
    private bool courseComplete = false;
    private int currentRoom = 0;
    private bool roomsComplete = false;

    //TODO: Sort out row/column issue, probably go back to xy and change all references
    // also fix orientation of walls to correspond to cardinal directions

    private void Start()
    {
        InitialiseMaze();
        CreateMaze();
        CreateRooms();
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

        int bail = 0;
        while (!roomsComplete)
        {
            VisitCellAndCheckConnection(currentCell, false, true);
            currentCell = GetNextUnvisitedCell();
            currentRoom++;
            Debug.Log("New room " + currentRoom);
            UpdateDebugCells();
            bail++;
            if (bail > 100)
                return;
        }
    }

    void UpdateDebugCells()
    {
        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                int cellNo = y * MazeY + x;
                Cells[cellNo] = MazeCells[x, y];
            }
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
            Debug.Log("Cell has already been visited");
            return;
        }

        MazeCell nextCell = null;
        // mark cell as visited so we dont come back
        Debug.Log("Checking cell " + cell.CellNumber.ToString() + ". Direction: " + dir + ". First cell? " + isFirstCellInRoom);
        // dir: false = east, true = north
        if (dir)
        {
            // north
            if(!cell.HasNorthWall)
            {
                Debug.Log("Cell doesnt have a north wall, check the cell above");
                cell.NorthSouthRoom = true;
                cell.visited = true;
                cell.roomNo = currentRoom;
                nextCell = MazeCells[cell.CellX, cell.CellY + 1];
                VisitCellAndCheckConnection(nextCell, dir, false);
            }
            else
            {
                Debug.Log("Cell has a north wall, stop");
                cell.visited = true;
                // wall here
                // if this is the first cell in room then it is a one cell room
                if (isFirstCellInRoom)
                {
                    Debug.Log("Cell is a single room");
                    cell.SingleCellRoom = true;
                }                    
            }
        }
        else
        {
            // east
            if(!cell.HasEastWall)
            {
                Debug.Log("Cell doesnt have an east wall, check the cell to the right");
                cell.EastWestRoom = true;
                cell.visited = true;
                cell.roomNo = currentRoom;
                nextCell = MazeCells[cell.CellX + 1, cell.CellY];
                VisitCellAndCheckConnection(nextCell, dir, false);
            }
            else
            {
                Debug.Log("Cell has an east wall");
                // there is a wall here, if this is the first cell in room, run again but looking north
                if (isFirstCellInRoom)
                {
                    Debug.Log("Checking for a passage north");
                    VisitCellAndCheckConnection(cell, !dir, isFirstCellInRoom);
                }
                cell.visited = true;
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
            roomsComplete = true;

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
        mazeCellCount = MazeX * MazeY;

        wallLength = Wall.transform.localScale.x;
        floorLength = Floor.transform.localScale.x;

        MazeCells = new MazeCell[MazeX, MazeY];
        // DEBUG
        Cells = new MazeCell[MazeX * MazeY];

        GameObject MazeCellHolder = new GameObject
        {
            name = "Maze Cells"
        };
        MazeCellHolder.transform.parent = transform;

        GameObject tempFloor, tempWall;

        for (int y = 0; y < MazeY; y++)
        {
            for (int x = 0; x < MazeX; x++)
            {
                int cellNo = y * MazeX + x;
                MazeCells[x, y] = new MazeCell
                {
                    CellNumber = cellNo,
                    CellX = x,
                    CellY = y
                };
                //Debug.Log("Cell " + cellNo + " created at (" + x + ", " + y + ")");
                GameObject CurrentCell = new GameObject
                {
                    name = "Maze Cell " + cellNo.ToString()
                };
                CurrentCell.transform.parent = MazeCellHolder.transform;
                
                // start with the floor
                tempFloor = Instantiate(Floor, new Vector3(x * floorLength, -(floorLength / 2f), y * floorLength), Quaternion.identity) as GameObject;
                MazeCells[x, y].Floor = tempFloor;
                MazeCells[x, y].Floor.name = "Floor " + x + "," + y;
                tempFloor.transform.parent = CurrentCell.transform;

                if (y == 0)
                {
                    tempWall = Instantiate(Wall, new Vector3(x * wallLength, 0, (y * wallLength) - (wallLength / 2f)), Quaternion.identity) as GameObject;
                    MazeCells[x, y].SouthWall = tempWall;
                    MazeCells[x, y].SouthWall.name = "South Wall " + x + "," + y;
                    MazeCells[x, y].HasSouthWall = true;
                    tempWall.transform.parent = CurrentCell.transform;
                }

                tempWall = Instantiate(Wall, new Vector3(x * wallLength, 0, (y * wallLength) + (wallLength / 2f)), Quaternion.identity) as GameObject;
                MazeCells[x, y].NorthWall = tempWall;
                MazeCells[x, y].NorthWall.name = "North Wall " + x + "," + y;
                MazeCells[x, y].HasNorthWall = true;
                tempWall.transform.parent = CurrentCell.transform;
                
                if (x == 0)
                {
                    tempWall = Instantiate(Wall, new Vector3((x * wallLength) - (wallLength / 2f), 0, y * wallLength), Quaternion.identity) as GameObject;
                    MazeCells[x, y].WestWall = tempWall;
                    MazeCells[x, y].WestWall.name = "West Wall " + x + "," + y;
                    MazeCells[x, y].HasWestWall = true;
                    MazeCells[x, y].WestWall.transform.Rotate(Vector3.up * 90f);
                    tempWall.transform.parent = CurrentCell.transform;
                }

                tempWall = Instantiate(Wall, new Vector3((x * wallLength) + (wallLength / 2f), 0, y * wallLength), Quaternion.identity) as GameObject;
                MazeCells[x, y].EastWall = tempWall;
                MazeCells[x, y].EastWall.name = "East Wall " + x + "," + y;
                MazeCells[x, y].HasEastWall = true;
                MazeCells[x, y].EastWall.transform.Rotate(Vector3.up * 90f);
                tempWall.transform.parent = CurrentCell.transform;

                // DEBUG PURPOSES
                Cells[cellNo] = MazeCells[x, y];
            }
        }
    }
}
