using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

	private PlayerHandler playerHandler;
	private List<EnemyHandler> enemyMeleeHandlerList;
	private List<EnemyRangedHandler> enemyRangedHandlerList;
	private List<EnemySlowerHandler> enemySlowerHandlerList;


	[SerializeField]
    private CameraFollow cameraFollow;

	private bool asStarted = false;

	GameObject[] enemySpawner;

	public LayerMask wallLayer;

	private void Start() 
	{
		enemyMeleeHandlerList = new List<EnemyHandler>();
		enemyRangedHandlerList = new List<EnemyRangedHandler>();
		enemySlowerHandlerList = new List<EnemySlowerHandler>();
		playerHandler = PlayerHandler.CreatePlayer(GetClosestEnemyHandler);

		cameraFollow.Setup(11.25f, playerHandler.GetPosition);

		enemySpawner = GameObject.FindGameObjectsWithTag("Spawners");

	}

	private void Update()
	{
		if (!asStarted) 
		{
			if (enemyMeleeHandlerList.Count > 0) asStarted = true;

		} else if (asStarted)
		{
			for(int i = enemyMeleeHandlerList.Count - 1; i > -1; i--)
 			{
    			if (enemyMeleeHandlerList[i] == null) enemyMeleeHandlerList.RemoveAt(i);
 			}
			for(int i = enemyRangedHandlerList.Count - 1; i > -1; i--)
			{
				if (enemyRangedHandlerList[i] == null)	enemyRangedHandlerList.RemoveAt(i);
			}
			for(int i = enemySlowerHandlerList.Count - 1; i > -1; i--)
			{
				if (enemySlowerHandlerList[i] == null)	enemySlowerHandlerList.RemoveAt(i);
			}
			for(int i = enemySpawner.Length - 1; i > -1; i--)
			{
				
			}  

			if (enemyMeleeHandlerList.Count == 0 && enemyRangedHandlerList.Count == 0 && 
				enemySlowerHandlerList.Count == 0 && enemySpawner == null) 
			{
				playerHandler.SaveRewards();
				StartCoroutine(WinMessage());
			}
		}
	}

	IEnumerator WinMessage()
	{
		GameObject objectWinText = GameObject.Find("WinText");
		Text WinText = objectWinText.GetComponent<Text>();
		WinText.enabled = true;
		yield return new WaitForSeconds(5);
		WinText.enabled = false;
		Restart();
		
	}

	public void SpawnMeleeEnemy() 
	{
		for (int i = 0; i < 30; i++)
		{
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2, 3f); 
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
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2, 3f); 
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
			Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2, 3f); 
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

	private void EnemyHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyHandler enemyMeleeHandler = sender as EnemyHandler;
		enemyMeleeHandlerList.Remove(enemyMeleeHandler);
		playerHandler.GetRewards(20, 10);
	}

	private void EnemyRangedHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyRangedHandler enemyRangedHandler = sender as EnemyRangedHandler;
		enemyRangedHandlerList.Remove(enemyRangedHandler);
		playerHandler.GetRewards(30, 7);
	}

	private void EnemySlowerHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemySlowerHandler enemySlowerHandler = sender as EnemySlowerHandler;
		enemySlowerHandlerList.Remove(enemySlowerHandler);
		playerHandler.GetRewards(10, 15);
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

	public static void Restart() 
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
