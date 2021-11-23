using UnityEngine;
using System.Collections;

public class FloorCollider : MonoBehaviour
{

	public GameObject parentFloor;
	public Material visitedMaterial;

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			parentFloor.GetComponent<Renderer> ().material = visitedMaterial;
		}
	}
}
