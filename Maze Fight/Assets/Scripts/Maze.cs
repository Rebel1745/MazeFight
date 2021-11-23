using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
	
	[System.Serializable]
	public class Cell
	{
		public bool visited = false;
		public GameObject north;//1
		public GameObject west;//2
		public GameObject east;//3
		public GameObject south;//4
		public GameObject floor;
	}

	public GameObject Wall;
	public GameObject Floor;
	public GameObject Player;
	private float wallLength = 0.0f;
	private float wallWidth = 0.0f;
	private float wallHeight = 0.0f;
	public int xSize = 5;
	public int ySize = 5;
	private Vector3 initialPos;
	private GameObject wallHolder;
	private GameObject floorHolder;	
	public Cell[] Cells;
	private int totalCells;
	private int currentCell = 0;
	private int visitedCells = 0;
	private bool startedBuilding = false;
	private int currentNeighbour = 0;
	private List<int> lastCells;
	private int backingUp = 0;
	private int wallToBreak = 0;
	public Material WallMaterial;
	public Material FloorMaterial;
	public Material FloorVisitedMaterial;

	// Use this for initialization
	void Start ()
	{
		totalCells = xSize * ySize;
		CreateWalls ();
	}
	
	void CreateWalls ()
	{
		
		wallHolder = new GameObject ();
		wallHolder.name = "MazeWalls";
		floorHolder = new GameObject ();
		floorHolder.name = "MazeFloors";
		
		wallWidth = Wall.transform.localScale.x;
		wallHeight = Wall.transform.localScale.y;
		wallLength = Wall.transform.localScale.z;
		
		initialPos = new Vector3 ((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
		Vector3 myPos = initialPos;
		Vector3 floorPos = initialPos;
		GameObject tempWall;
		GameObject tempFloor;
		
		//x axis
		for (int i = 0; i < ySize; i++) {
			for (int j = 0; j <= xSize; j++) {
				myPos = new Vector3 (initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
				tempWall = Instantiate (Wall, myPos, Quaternion.identity) as GameObject;
				tempWall.GetComponent<Renderer> ().material = WallMaterial;
				tempWall.transform.parent = wallHolder.transform;
				// create floor
				if (j < xSize) {
					floorPos = new Vector3 (myPos.x + wallLength / 2, myPos.y - wallHeight / 2, myPos.z);
					tempFloor = Instantiate (Floor, floorPos, Quaternion.identity) as GameObject;
					tempFloor.transform.localScale = new Vector3 (tempFloor.transform.localScale.x * wallLength, tempFloor.transform.localScale.y, tempFloor.transform.localScale.z * wallLength);
					tempFloor.GetComponent<Renderer> ().material = FloorMaterial;
					tempFloor.transform.parent = floorHolder.transform;
				}
			}
		}
		
		//y axis
		for (int i = 0; i <= ySize; i++) {
			for (int j = 0; j < xSize; j++) {
				myPos = new Vector3 (initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
				tempWall = Instantiate (Wall, myPos, Quaternion.Euler (0.0f, 90.0f, 0.0f)) as GameObject;
				tempWall.GetComponent<Renderer> ().material = WallMaterial;
				tempWall.transform.parent = wallHolder.transform;
			}
		}
		
		//CreateFloor ();
		CreateCells ();
        CreatePlayer();
    }

    void CreatePlayer()
    {
        Vector3 playerPos = new Vector3(initialPos.x, initialPos.y, initialPos.z - wallLength / 2);
        Player = Instantiate(Player, playerPos, Quaternion.identity) as GameObject;
        Player.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void CreateCells ()
	{
		lastCells = new List<int> ();
		lastCells.Clear ();
		GameObject[] allWalls;
		GameObject[] allFloors;
		int childrenWalls = wallHolder.transform.childCount;
		int childrenFloors = floorHolder.transform.childCount;
		allWalls = new GameObject[childrenWalls];
		allFloors = new GameObject[childrenFloors];
		Cells = new Cell[totalCells];
		int eastWestProcess = 0;
		int childProcess = 0;
		int termCount = 0;
		
		// get all the children
		for (int i = 0; i < childrenWalls; i++) {
			allWalls [i] = wallHolder.transform.GetChild (i).gameObject;
		}
		// get all the children
		for (int i = 0; i < childrenFloors; i++) {
			allFloors [i] = floorHolder.transform.GetChild (i).gameObject;
		}
		
		// assigns walls to the cells
		for (int cellProcess = 0; cellProcess<Cells.Length; cellProcess++) {
		
			if (termCount == xSize) {
				eastWestProcess++;
				termCount = 0;
			}
			
			Cells [cellProcess] = new Cell ();
			Cells [cellProcess].west = allWalls [eastWestProcess];
			Cells [cellProcess].south = allWalls [childProcess + (xSize + 1) * ySize];
			
			eastWestProcess++;
			
			termCount++;
			childProcess++;
			
			Cells [cellProcess].east = allWalls [eastWestProcess];
			Cells [cellProcess].north = allWalls [(childProcess + (xSize + 1) * ySize) + xSize - 1];
			Cells [cellProcess].floor = allFloors [cellProcess];
		}
		
		CreateMaze ();
	}
	
	void CreateMaze ()
	{
		while (visitedCells < totalCells) {
			if (startedBuilding) {
				RandomNeighbour ();
				if (!Cells [currentNeighbour].visited && Cells [currentCell].visited) {
					BreakWall ();
					Cells [currentNeighbour].visited = true;
					visitedCells++;
					lastCells.Add (currentCell);
					currentCell = currentNeighbour;
					if (lastCells.Count > 0) {
						backingUp = lastCells.Count - 1;
					}
				}
			} else {
				currentCell = Random.Range (0, totalCells);
				Cells [currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
		}
	}
	
	void BreakWall ()
	{
		switch (wallToBreak) {
		case 1:
			Destroy (Cells [currentCell].north);
			break;
		case 2:
			Destroy (Cells [currentCell].west);
			break;
		case 3:
			Destroy (Cells [currentCell].east);
			break;
		case 4:
			Destroy (Cells [currentCell].south);
			break;
		}
	}
	
	void RandomNeighbour ()
	{
		int length = 0;
		// pos neighbours (top,bottom,left,right)
		int[] neighbours = new int[4];
		int[] connectingWall = new int[4];
		int check = 0;
		check = (currentCell + 1) / xSize;
		check -= 1;
		check *= xSize;
		check += xSize;
		
		//east wall
		if (currentCell + 1 < totalCells && (currentCell + 1) != check) {
			if (!Cells [currentCell + 1].visited) {
				neighbours [length] = currentCell + 1;
				connectingWall [length] = 3;
				length++;
			}
		}
		
		//west wall
		if (currentCell - 1 >= 0 && currentCell != check) {
			if (!Cells [currentCell - 1].visited) {
				neighbours [length] = currentCell - 1;
				connectingWall [length] = 2;
				length++;
			}
		}
		
		//north wall
		if (currentCell + xSize < totalCells) {
			if (!Cells [currentCell + xSize].visited) {
				neighbours [length] = currentCell + xSize;
				connectingWall [length] = 1;
				length++;
			}
		}
		
		//south wall
		if (currentCell - xSize >= 0) {
			if (!Cells [currentCell - xSize].visited) {
				neighbours [length] = currentCell - xSize;
				connectingWall [length] = 4;
				length++;
			}
		}
		
		if (length != 0) {
			int chosen = Random.Range (0, length);
			currentNeighbour = neighbours [chosen];
			wallToBreak = connectingWall [chosen];
		} else {
			if (backingUp > 0) {
				currentCell = lastCells [backingUp];
				backingUp--;
			}
		}
	}
}
