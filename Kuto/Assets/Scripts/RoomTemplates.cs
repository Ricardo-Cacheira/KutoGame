using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour {

	public GameObject[] bottomRooms;
	public GameObject[] topRooms;
	public GameObject[] leftRooms;
	public GameObject[] rightRooms;

	public GameObject closedRooms;

	public List<GameObject> rooms;

	public float waitTime;
	public bool spawnedBoss;
	public GameObject boss;
	public GameObject reward;

	void Update()
	{
		if (waitTime <= 0 && !spawnedBoss)
		{
			for (int i = 0; i < rooms.Count; i++)
			{
				if (i == rooms.Count - 1)
				{
					if (rooms[i].CompareTag("RoomClosed"))
					{
						rooms.RemoveAt(i);
					} else {
						Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
						Instantiate(reward, rooms[i].transform.position + (Vector3.right * 1.2f), Quaternion.identity);
						spawnedBoss = true;
					}
				}
			}
		} else {
			waitTime -= Time.deltaTime;
		}
	}
}
