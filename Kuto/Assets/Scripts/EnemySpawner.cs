using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public GameObject enemyPrefab;
	public GameObject enemiesGroup;
	
	void Update () {
		if (enemiesGroup.transform.childCount < 10) 
		{
			GameObject newEnemy = Instantiate(enemyPrefab, new Vector2(0, 0), enemiesGroup.transform.rotation);
			newEnemy.transform.parent = enemiesGroup.transform;
		}
	}
}
