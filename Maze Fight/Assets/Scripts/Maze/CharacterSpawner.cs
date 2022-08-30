using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public MazeGenerator mg;
    public GameObject PlayerPrefab;
    public GameObject CameraFollowTargetPrefab;
    public CameraFollow cf;
    public GameObject[] EnemyPrefabs;
    public int EnemyNumber = 5;

    private void Awake()
    {
        cf = Camera.main.GetComponent<CameraFollow>();
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
    }

    public void CreateEnemies()
    {
        for(int i = 0; i <= EnemyNumber; i++)
        {
            float spawnX = Random.Range(-mg.floorLength / 2f, mg.floorLength / 2f);
            float spawnZ = Random.Range(-mg.floorLength / 2f, mg.floorLength / 2f);
            int randomEnemy = Random.Range(0, EnemyPrefabs.Length);

            Vector3 enemySpawnPos = new Vector3(mg.MazeCells[2, 0].Floor.transform.position.x + spawnX, mg.MazeCells[2, 0].Floor.transform.position.y, mg.MazeCells[2, 0].Floor.transform.position.z + spawnZ);
            GameObject testEnemy = Instantiate(EnemyPrefabs[randomEnemy], enemySpawnPos, Quaternion.identity);
        }
        
    }
}
