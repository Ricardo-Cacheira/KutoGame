using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawn : MonoBehaviour {
	public GameObject gameHandle;
	private GameHandler gameHandler;
	public float repeatRate = 5.5f;
	private bool wasActivated;

	void Start () {
		gameHandler = gameHandle.GetComponent<GameHandler>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") && !wasActivated)
		{
			wasActivated = true;

			InvokeRepeating("MeleeSpawner", 2f, repeatRate);
			InvokeRepeating("RangedSpawner", 3.5f, repeatRate * 1.75f);
			
			Destroy(gameObject, 60f);
		}
	}

	private void MeleeSpawner()
	{
		gameHandler.SpawnMeleeEnemy();
	}

	private void RangedSpawner()
	{
		gameHandler.SpawnRangedEnemy();
	} 
}
