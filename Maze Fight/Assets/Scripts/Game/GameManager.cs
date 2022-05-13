using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform MazeLoader;
    public MazeGenerator mg;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject CameraFollowTargetPrefab;
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
        GameObject camFollowTarget = Instantiate(CameraFollowTargetPrefab, playerSpawnPos, Quaternion.identity);
        camFollowTarget.GetComponent<CameraFollowTarget>().pl = player;
        camFollowTarget.GetComponent<CameraFollowTarget>().pm = player.GetComponent<PlayerInputMovement>();
        cf.FollowTarget = camFollowTarget.transform;
        cf.pm = player.GetComponent<PlayerInputMovement>();

        // enemy spawn
        Vector3 enemySpawnPos = new Vector3(mg.MazeCells[2, 0].Floor.transform.position.x, mg.MazeCells[2, 0].Floor.transform.position.y, mg.MazeCells[2, 0].Floor.transform.position.z);
        GameObject testEnemy = Instantiate(EnemyPrefab, enemySpawnPos, Quaternion.identity);
        testEnemy.GetComponent<EnemyMovement>().Player = player.transform;
    }
}
