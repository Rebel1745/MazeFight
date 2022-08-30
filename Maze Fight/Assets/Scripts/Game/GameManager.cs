using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform MazeLoader;
    public MazeGenerator mg;
    public int MazeSeed = 0;

    private void Awake()
    {
        mg = MazeLoader.GetComponent<MazeGenerator>();
    }

    private void Start()
    {
        Random.InitState(MazeSeed);
        mg.GenerateMaze();
    }
}
