using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MazeCell
{
    public int CellNumber = 0;
    public int CellX = 0;
    public int CellY = 0;
    public Transform CellHolder;
    public bool visited = false;
    public bool HasNorthWall = false;
    public GameObject NorthWall; // 1
    public bool HasSouthWall = false;
    public GameObject SouthWall; // 2
    public bool HasEastWall = false;
    public GameObject EastWall; // 3
    public bool HasWestWall = false;
    public GameObject WestWall; // 4
    public GameObject Floor;
    public int roomNo = -1;
    public bool EastWestRoom = false;
    public bool NorthSouthRoom = false;
    public bool SingleCellRoom = false;
}
