using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform MazeLoader;
    public MazeGenerator mg;
    public GameObject PlayerPrefab;
    public CameraFollow cf;

    private void Awake()
    {
        mg = MazeLoader.GetComponent<MazeGenerator>();
        cf = Camera.main.GetComponent<CameraFollow>();
    }

    private void Start()
    {
        mg.GenerateMaze();
        Vector3 playerSpawnPos = new Vector3(mg.MazeCells[0, 0].Floor.transform.position.x, mg.MazeCells[0, 0].Floor.transform.position.y + 1.5f, mg.MazeCells[0, 0].Floor.transform.position.z);
        GameObject player = Instantiate(PlayerPrefab, playerSpawnPos, Quaternion.identity);
        cf.FollowTarget = player.GetComponent<PlayerInputMovement>().FollowTarget;
    }
}
