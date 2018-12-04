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
	private CameraShaker cameraShaker;
	private GameObject cameraFollow;
	private bool wasActivated = false;
	private bool spawnTimeEnded, finishedSpawning;
	private float randMelee, randRanged, randSlower, randSpawnTime;
	private int randNumBomb, randNumMissile;


	void Start () {
		gameHandle = GameObject.Find("GameManager");
		gameHandler = gameHandle.GetComponent<GameHandler>();

		cameraFollow = GameObject.Find("CameraFollow");

		cameraShaker = (CameraShaker) FindObjectOfType(typeof (CameraShaker)); 
		cameraShaker.enabled = false;
		
		door = this.transform.GetChild(0).gameObject;
		doorRenderer = door.GetComponent<TilemapRenderer>();
		doorCollider = door.GetComponent<TilemapCollider2D>();

		gameHandler.numOfSpawners++;
	}

	void Update()
	{
		if (finishedSpawning && GameHandler.noEnemies) {
			doorRenderer.enabled = false;
			doorCollider.enabled = false;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") && !wasActivated)
		{
			doorRenderer.enabled = true;
			doorCollider.enabled = true;
			
			wasActivated = true;

			StartCoroutine(ShakeScreen());

			randMelee = Random.Range(6, 10);
			randRanged = Random.Range(8, 11);
			randSlower = Random.Range(7.5f, 10);
			randSpawnTime = Random.Range(16, 27);
			randNumBomb = Random.Range(2, 4);
			randNumMissile = Random.Range(1, 3);

			InvokeRepeating("MeleeSpawner", 1f, randMelee);
			InvokeRepeating("RangedSpawner", 6f, randRanged);
			InvokeRepeating("SlowerSpawner", 4f, randSlower);

			for (int i = 0; i < randNumBomb; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-9.5f, 9.5f), Random.Range(-4.5f, 4.5f));
				Instantiate(GameAssets.i.pfBomb, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			for (int i = 0; i < randNumMissile; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-9.5f, 9.5f), Random.Range(-4.5f, 4.5f));
				Instantiate(GameAssets.i.pfMissile, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			
			StartCoroutine(DestroySelf(randSpawnTime));
		}
	}

	IEnumerator DestroySelf(float time)
	{
		yield return new WaitForSeconds(time);
		gameHandler.numOfSpawners--;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		finishedSpawning = true;
		wasActivated = false;
	}

	IEnumerator ShakeScreen()
	{
		cameraFollow.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
		cameraShaker.shakeDuration = 1f;
		cameraShaker.enabled = true;
		gameHandler.playerHandler.speed = 0;
		yield return new WaitForSeconds(1);
		gameHandler.playerHandler.speed = 6f;
		cameraShaker.enabled = false;
	}

	private void MeleeSpawner()
	{
		if (wasActivated) gameHandler.SpawnMeleeEnemy();
	}

	private void RangedSpawner()
	{
		if (wasActivated) gameHandler.SpawnRangedEnemy();
	}

	private void SlowerSpawner()
	{
		if (wasActivated) gameHandler.SpawnSlowerEnemy();
	} 
}
