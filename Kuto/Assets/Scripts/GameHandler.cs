﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour {

	public PlayerHandler playerHandler;
	private List<EnemyHandler> enemyMeleeHandlerList;
	private List<EnemyRangedHandler> enemyRangedHandlerList;
	private List<EnemySlowerHandler> enemySlowerHandlerList;

	[SerializeField]
    private CameraFollow cameraFollow;

	private bool asStarted = false;

	GameObject[] enemySpawner;
	GameObject enemySpawnerBeach;

	public LayerMask wallLayer;

	private int timeToSurvive;
	private float loading = 1;

	public int numOfSpawners;
	public static bool noEnemies;
	bool asPlayed;
	bool asDied;
	int lvl;

	private void Start() 
	{
		enemyMeleeHandlerList = new List<EnemyHandler>();
		enemyRangedHandlerList = new List<EnemyRangedHandler>();
		enemySlowerHandlerList = new List<EnemySlowerHandler>();

		playerHandler = PlayerHandler.CreatePlayer();
		playerHandler.OnDead += PlayerHandler_OnDead;

		cameraFollow.Setup(playerHandler.GetPosition);

		enemySpawner = GameObject.FindGameObjectsWithTag("Spawners");
		enemySpawnerBeach = GameObject.Find("EnemySpawnerBeach");

		numOfSpawners = enemySpawner.Length;

		lvl = GameControl.control.lvl;
	}

	private void Update()
	{
		if (!asStarted) 
		{
			loading -= Time.deltaTime;
			if (loading <= 0) asStarted = true;

		} else if (asStarted)
		{
			for(int i = enemyMeleeHandlerList.Count - 1; i > -1; i--)
    			if (enemyMeleeHandlerList[i] == null) enemyMeleeHandlerList.RemoveAt(i);
 			
			for(int i = enemyRangedHandlerList.Count - 1; i > -1; i--)
				if (enemyRangedHandlerList[i] == null)	enemyRangedHandlerList.RemoveAt(i);

			for(int i = enemySlowerHandlerList.Count - 1; i > -1; i--)
				if (enemySlowerHandlerList[i] == null)	enemySlowerHandlerList.RemoveAt(i);

			if (enemyMeleeHandlerList.Count == 0 && enemyRangedHandlerList.Count == 0 && enemySlowerHandlerList.Count == 0) 
			{
				noEnemies = true;
				if (numOfSpawners == 0 || enemySpawnerBeach == null) 
				{
					// playerHandler.SaveRewards();
					if (!asPlayed) 
					{
						StartCoroutine(WinMessage());
						asPlayed = true;
					}
				}
			} else 
			{
				noEnemies = false;
			}
		}
	}

	public void SpawnMeleeEnemy() 
	{
		for (int i = 0; i < 30; i++)
		{
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2f, 3f); 
			Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1, wallLayer);
			if (colliders.Length == 0)
			{
				EnemyHandler enemyHandler = EnemyHandler.CreateEnemy(spawnPosition, playerHandler);
				enemyHandler.OnDead += EnemyHandler_OnDead;
				enemyMeleeHandlerList.Add(enemyHandler);
				break;
			}
		}
	}

	public void SpawnRangedEnemy() 
	{
		for (int i = 0; i < 30; i++)
		{
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2.5f, 3f); 
			Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1, wallLayer);
			if (colliders.Length == 0)
			{
				EnemyRangedHandler enemyRangedHandler = EnemyRangedHandler.CreateEnemy(spawnPosition, playerHandler);
				enemyRangedHandler.OnDead += EnemyRangedHandler_OnDead;
				enemyRangedHandlerList.Add(enemyRangedHandler);
				break;
			}
		}
	}

	public void SpawnSlowerEnemy() 
	{
		for (int i = 0; i < 30; i++)
		{
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2f, 2.5f); 
			Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1, wallLayer);
			if (colliders.Length == 0)
			{
				EnemySlowerHandler enemySlowerHandler = EnemySlowerHandler.CreateEnemy(spawnPosition, playerHandler);
				enemySlowerHandler.OnDead += EnemySlowerHandler_OnDead;
				enemySlowerHandlerList.Add(enemySlowerHandler);
				break;
			}
		}
	}

	public void SpawnBoss() 
	{
		BossHandler bossHandler = BossHandler.CreateEnemy(new Vector3(playerHandler.transform.position.x, playerHandler.transform.position.y + 5, playerHandler.transform.position.z), playerHandler);
		bossHandler.OnDead += BossHandler_OnDead;
	}

	private void PlayerHandler_OnDead(object sender, System.EventArgs e) 
	{
		PlayerHandler playerHandler = sender as PlayerHandler;
		Restart();
	}

	private void EnemyHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyHandler enemyMeleeHandler = sender as EnemyHandler;
		FindObjectOfType<AudioManager>().Play("SlashEnemyKill");
		enemyMeleeHandlerList.Remove(enemyMeleeHandler);
		playerHandler.GetRewards(20 + lvl, 10 + lvl);
		Instantiate(GameAssets.i.psMelee, enemyMeleeHandler.transform.position, Quaternion.identity);
	}

	private void EnemyRangedHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyRangedHandler enemyRangedHandler = sender as EnemyRangedHandler;
		enemyRangedHandler.GetComponent<ParticleSystem>().Play();
		FindObjectOfType<AudioManager>().Play("SlashEnemyKill");
		enemyRangedHandlerList.Remove(enemyRangedHandler);
		playerHandler.GetRewards(30 + lvl, 7 + lvl);
		Instantiate(GameAssets.i.psRanged, enemyRangedHandler.transform.position, Quaternion.identity);

	}

	private void EnemySlowerHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemySlowerHandler enemySlowerHandler = sender as EnemySlowerHandler;
		FindObjectOfType<AudioManager>().Play("SlashEnemyKill");
		enemySlowerHandlerList.Remove(enemySlowerHandler);
		playerHandler.GetRewards(10 + lvl, 15 + lvl);
		Instantiate(GameAssets.i.psSlow, enemySlowerHandler.transform.position, Quaternion.identity);

	}

	private void BossHandler_OnDead(object sender, System.EventArgs e) 
	{
		BossHandler bossHandler = sender as BossHandler;
		FindObjectOfType<AudioManager>().Stop("Boss");
		playerHandler.GetRewards(500 + (lvl * 10), 0);
		GameControl.control.isXpMax = false;
		playerHandler.xp++;
		playerHandler.SaveRewards();
		GameControl.control.CalculateLevel();
		GameControl.control.Save();
		StartCoroutine(WinMessage());
		FindObjectOfType<AudioManager>().Play("MissionSuccess");
		bool asPlayed = false;
		if (!asPlayed){
			StartCoroutine(PlayGuessYouWin());
			asPlayed = true;
		}

		Instantiate(GameAssets.i.psBoss, bossHandler.transform.position, Quaternion.identity);
	}

	private EnemyHandler GetClosestEnemyHandler(Vector3 playerPosition) 
	{
		const float maxDistance = 50f;
		EnemyHandler closest = null;
		foreach (EnemyHandler enemyMeleeHandler in enemyMeleeHandlerList) 
		{
			if (Vector3.Distance(playerPosition, enemyMeleeHandler.GetPosition()) > maxDistance) continue;
			
			if (closest == null) 
			{
				closest = enemyMeleeHandler;

			} else 
			{
				if (Vector3.Distance(playerPosition, enemyMeleeHandler.GetPosition()) < Vector3.Distance(playerPosition, closest.GetPosition())) 
				{
					closest = enemyMeleeHandler;
				}
			}
		}
		return closest;
	}

	private EnemyRangedHandler GetClosestEnemyRangedHandler(Vector3 playerPosition) 
	{
		const float maxDistance = 50f;
		EnemyRangedHandler closest = null;
		foreach (EnemyRangedHandler enemyRangedHandler in enemyRangedHandlerList) 
		{
			if (Vector3.Distance(playerPosition, enemyRangedHandler.GetPosition()) > maxDistance) continue;
			
			if (closest == null) 
			{
				closest = enemyRangedHandler;

			} else 
			{
				if (Vector3.Distance(playerPosition, enemyRangedHandler.GetPosition()) < Vector3.Distance(playerPosition, closest.GetPosition())) 
				{
					closest = enemyRangedHandler;
				}
			}
		}
		return closest;
	}

	private void Restart() 
	{
		PlayerHandler.playerHandler.SaveRewards();
		if (!asDied) StartCoroutine(LoseMessage());
	}

	public static IEnumerator WinMessage()
	{
		if (SceneManager.GetActiveScene().name == "BeachScene") FindObjectOfType<AudioManager>().Stop("Beach");
		GameObject objectWinText = GameObject.Find("WinText");
		Text WinText = objectWinText.GetComponent<Text>();
		WinText.enabled = true;
		FindObjectOfType<AudioManager>().Play("MissionSuccess");
		yield return new WaitForSeconds(6.5f);
		WinText.enabled = false;
		PlayerHandler.playerHandler.SaveRewards();
		SceneManager.LoadScene(1);
	}
	
	private IEnumerator PlayGuessYouWin()
	{
		yield return new WaitForSeconds(.7f);
		FindObjectOfType<AudioManager>().Play("GuessYouWin");
	}

	private IEnumerator LoseMessage()
	{
		AudioManager am = FindObjectOfType<AudioManager>();
		if (SceneManager.GetActiveScene().name == "BeachScene") am.Stop("Beach");
		if (SceneManager.GetActiveScene().name == "BossScene") am.Stop("Boss");
		if (SceneManager.GetActiveScene().name == "Prototype") 
		{
			am.Stop("JungleFight");
			am.Stop("Jungle");
		}
		asDied = true;
		GameObject objectLoseText = GameObject.Find("LoseText");
		Text LoseText = objectLoseText.GetComponent<Text>();
		LoseText.enabled = true;
		FindObjectOfType<AudioManager>().Play("MissionFail");
		yield return new WaitForSeconds(3.5f);
		LoseText.enabled = false;
		SceneManager.LoadScene(1);

	}
}	
