using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public MazeGenerator mg;
    public GameObject PlayerPrefab;
    public GameObject CameraFollowTargetPrefab;
    public MinimapCamera Minimap;
    public GameObject TrainingDummyPrefab;
    public CameraFollow cf;
    public GameObject[] EnemyPrefabs;
    public int EnemyNumber = 5;

    private void Awake()
    {
        cf = Camera.main.GetComponentInParent<CameraFollow>();
    }

    public void CreateTrainingDummy()
    {
        Vector3 dummySpawnPoint = new Vector3(mg.MazeCells[1, 0].Floor.transform.position.x, mg.MazeCells[1, 0].Floor.transform.position.y, mg.MazeCells[1, 0].Floor.transform.position.z);
        GameObject dummy = Instantiate(TrainingDummyPrefab, dummySpawnPoint, Quaternion.Euler(0f,-90f,0f));
    }

    public void CreatePlayer()
    {
        Vector3 playerSpawnPos = new Vector3(mg.MazeCells[0, 0].Floor.transform.position.x, mg.MazeCells[0, 0].Floor.transform.position.y + 1.5f, mg.MazeCells[0, 0].Floor.transform.position.z);
        GameObject player = Instantiate(PlayerPrefab, playerSpawnPos, Quaternion.identity);
        GameObject camFollowTarget = Instantiate(CameraFollowTargetPrefab, playerSpawnPos, Quaternion.identity);
        camFollowTarget.GetComponent<CameraFollowTarget>().pl = player;
        camFollowTarget.GetComponent<CameraFollowTarget>().pm = player.GetComponent<PlayerInputMovement>();
        cf.FollowTarget = camFollowTarget.transform;
        cf.pm = player.GetComponent<PlayerInputMovement>();

        // create the minimap
        Minimap.FollowTarget = camFollowTarget.transform;
    }

    public void CreateEnemies()
    {
        MazeCell currentCell;
        GameObject currentFloor;

        for (int y = 0; y < mg.MazeY; y++)
        {
            for (int x = 0; x < mg.MazeX; x++)
            {
                currentCell = mg.MazeCells[x, y];

                if (currentCell.roomNo != 0 && currentCell.EastWestRoom)
                {
                    currentFloor = currentCell.Floor;

                    for (int i = 0; i <= EnemyNumber; i++)
                    {
                        float spawnX = Random.Range(-mg.floorLength / 2f, mg.floorLength / 2f);
                        float spawnZ = Random.Range(-mg.floorLength / 2f, mg.floorLength / 2f);
                        int randomEnemy = Random.Range(0, EnemyPrefabs.Length);

                        Vector3 enemySpawnPos = new Vector3(currentCell.Floor.transform.position.x + spawnX, currentCell.Floor.transform.position.y, currentCell.Floor.transform.position.z + spawnZ);
                        GameObject enemy = Instantiate(EnemyPrefabs[randomEnemy], enemySpawnPos, Quaternion.identity);
                        enemy.transform.parent = currentCell.Enemies;
                    }
                }
            }
        }
        
    }
}
