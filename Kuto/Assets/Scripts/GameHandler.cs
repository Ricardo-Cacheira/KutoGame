using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public class GameHandler : MonoBehaviour {

	private PlayerHandler playerHandler;
	private List<EnemyHandler> enemyMeleeHandlerList;
	private List<EnemyRangedHandler> enemyRangedHandlerList;

	[SerializeField]
    private CameraFollow cameraFollow;

	private void Start() 
	{
		enemyMeleeHandlerList = new List<EnemyHandler>();
		enemyRangedHandlerList = new List<EnemyRangedHandler>();
		playerHandler = PlayerHandler.CreatePlayer(GetClosestEnemyHandler);

		cameraFollow.Setup(7.5f, playerHandler.GetPosition);
		
		FunctionPeriodic.Create(SpawnMeleeEnemy, 5f);
		FunctionPeriodic.Create(SpawnRangedEnemy, 8f);
	}

	private void SpawnMeleeEnemy() 
	{
		Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(2, 3f); 
		EnemyHandler enemyHandler = EnemyHandler.CreateEnemy(spawnPosition, playerHandler);
		enemyHandler.OnDead += EnemyHandler_OnDead;
		enemyMeleeHandlerList.Add(enemyHandler);
	}

	private void SpawnRangedEnemy() 
	{
		Vector3 spawnPosition = playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(6, 7.5f); 
		EnemyRangedHandler enemyRangedHandler = EnemyRangedHandler.CreateEnemy(spawnPosition, playerHandler);
		enemyRangedHandler.OnDead += EnemyHandler_OnDead;
		enemyRangedHandlerList.Add(enemyRangedHandler);
	}

	private void EnemyHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyHandler enemyMeleeHandler = sender as EnemyHandler;
		enemyMeleeHandlerList.Remove(enemyMeleeHandler);
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

	private void EnemyRangedHandler_OnDead(object sender, System.EventArgs e) 
	{
		EnemyRangedHandler enemyRangedHandler = sender as EnemyRangedHandler;
		enemyRangedHandlerList.Remove(enemyRangedHandler);
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