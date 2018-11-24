﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerBeach : MonoBehaviour {

	private GameObject gameHandle;
	private GameHandler gameHandler;
	private GameObject gameAssetsObject;
	private GameAssets gameAssets;
	private bool wasActivated = false;
	private bool spawnTimeEnded;
	private float randMelee, randRanged, randSlower, spawnTime;
	private int randNumBomb, randNumMissile;

	public Text timerText;
	public float timeCounter;


	void Start () {
		gameHandle = GameObject.Find("GameManager");
		gameHandler = gameHandle.GetComponent<GameHandler>();

		timeCounter = 5;
	}

	void Update()
	{
		if (timeCounter >= 1) {
			timeCounter -= Time.deltaTime;
			var minutes = Mathf.FloorToInt(timeCounter / 60);
			var seconds = Mathf.FloorToInt(timeCounter % 60).ToString("00");
			timerText.text = minutes + " : " + seconds;
		} else {
			timerText.text = "Clear the arena";
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") && !wasActivated)
		{
			wasActivated = true;

			randMelee = Random.Range(5, 13);
			randRanged = Random.Range(7, 11);
			randSlower = Random.Range(6.5f, 15);
			spawnTime = 60;
			randNumBomb = Random.Range(7, 18);
			randNumMissile = Random.Range(9, 17);

			InvokeRepeating("MeleeSpawner", 2f, randMelee);
			InvokeRepeating("RangedSpawner", 6f, randRanged);
			InvokeRepeating("SlowerSpawner", 4f, randSlower);

			for (int i = 0; i < randNumBomb; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-53, 53f), Random.Range(-46f, 46f) + 3);
				Instantiate(GameAssets.i.pfBomb, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			for (int i = 0; i < randNumMissile; i++)
			{
				Vector2 randPos = new Vector2(Random.Range(-53, 53f), Random.Range(-46f, 46f) + 3);
				Instantiate(GameAssets.i.pfMissile, new Vector2(this.transform.position.x + randPos.x, this.transform.position.y + randPos.y), Quaternion.identity);
			}
			
			Destroy(gameObject, timeCounter);
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

	private void SlowerSpawner()
	{
		gameHandler.SpawnSlowerEnemy();
	} 
}
