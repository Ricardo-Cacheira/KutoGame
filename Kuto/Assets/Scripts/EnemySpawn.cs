using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawn : MonoBehaviour {
	private GameObject gameHandle;
	private GameHandler gameHandler;
	private GameObject gameAssetsObject;
	private GameAssets gameAssets;
	private GameObject door;
	private TilemapRenderer doorRenderer;
	private TilemapCollider2D doorCollider;
	private bool wasActivated = false;
	private bool spawnTimeEnded;
	private float randMelee, randRanged, randSlower, randSpawnTime;
	private int randNumBomb, randNumMissile;


	void Start () {
		gameHandle = GameObject.Find("GameManager");
		gameHandler = gameHandle.GetComponent<GameHandler>();

		door = this.transform.GetChild(0).gameObject;
		doorRenderer = door.GetComponent<TilemapRenderer>();
		doorCollider = door.GetComponent<TilemapCollider2D>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") && !wasActivated)
		{
			doorRenderer.enabled = true;
			doorCollider.enabled = true;
			
			wasActivated = true;

			randMelee = Random.Range(6, 10);
			randRanged = Random.Range(8, 11);
			randSlower = Random.Range(7.5f, 10);
			randSpawnTime = Random.Range(16, 27);
			randNumBomb = Random.Range(2, 4);
			randNumMissile = Random.Range(1, 3);

			InvokeRepeating("MeleeSpawner", 1f, randMelee);
			InvokeRepeating("RangedSpawner", 4f, randRanged);
			InvokeRepeating("SlowerSpawner", 2f, randSlower);

			for (int i = 0; i < randNumBomb; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-10.5f, 10.5f), Random.Range(-5.5f, 5.5f));
				Instantiate(GameAssets.i.pfBomb, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			for (int i = 0; i < randNumMissile; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-10.5f, 10.5f), Random.Range(-5.5f, 5.5f));
				Instantiate(GameAssets.i.pfMissile, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			
			
			
			StartCoroutine(DestroySelf(randSpawnTime));
		}
	}

	IEnumerator DestroySelf(float time)
	{
		yield return new WaitForSeconds(time);
		doorRenderer.enabled = false;
		doorCollider.enabled = false;
		Destroy(gameObject);
	}

	private void MeleeSpawner()
	{
		gameHandler.SpawnMeleeEnemy();
	}

	private void RangedSpawner()
	{
		gameHandler.SpawnRangedEnemy();
	}

	private void SlowerSpawner()
	{
		gameHandler.SpawnSlowerEnemy();
	} 
}
