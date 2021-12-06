using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameManager gm;

    public void SpawnPlayer()
    {
        Instantiate(PlayerPrefab, gm.mg.MazeCells[0, 0].Floor.transform.position, Quaternion.identity);
    }
}
