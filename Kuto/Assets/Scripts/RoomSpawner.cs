using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

	public int openingDirection;
	//1 -> botexit
	//2 -> topexit
	//3 -> leftexit
	//4 -> rightexit

	private RoomTemplates templates;
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start()
	{
		Destroy(gameObject, waitTime);
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		Invoke("Spawn", 0.2f);
	}

	void Spawn()
	{
		if (spawned == false)
		{
			if (openingDirection == 2)
			{
				rand = Random.Range(0, templates.bottomRooms.Length - 1);
 				Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
		
			} else if (openingDirection == 1)
			{
				rand = Random.Range(0, templates.topRooms.Length - 1);
 				Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);

			} else if (openingDirection == 4)
			{
				rand = Random.Range(0, templates.leftRooms.Length - 1);
 				Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);

			} else if (openingDirection == 3)
			{
				rand = Random.Range(0, templates.rightRooms.Length - 1);
 				Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);

			}
			spawned = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("SpawnPoint"))
		{
			if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
			{
				Instantiate(templates.closedRooms, transform.position, Quaternion.identity);
				Destroy(gameObject);
			}
			spawned = true;
		}
	}
}
