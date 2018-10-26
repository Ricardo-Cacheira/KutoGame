using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
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

	public static GameHandler gh;


	public int gold;
	public float vitality;
	public float strength;
	public float experience;
	
	Scene scene;
  	string sceneName;

	void Awake()
	{
		if(gh == null)
		{
			DontDestroyOnLoad(gameObject);
			gh = this;
		}else if(gh != this)
		{
			Destroy(gameObject);
		}
	}

	private void Start() 
	{
		scene = SceneManager.GetActiveScene();
		sceneName = scene.name;

		if(sceneName != "town")
		{
			enemyMeleeHandlerList = new List<EnemyHandler>();
			enemyRangedHandlerList = new List<EnemyRangedHandler>();
			playerHandler = PlayerHandler.CreatePlayer(GetClosestEnemyHandler);

			cameraFollow.Setup(7.5f, playerHandler.GetPosition);
			
			FunctionPeriodic.Create(SpawnMeleeEnemy, 5f);
			FunctionPeriodic.Create(SpawnRangedEnemy, 8f);
		}else{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i).gameObject;
				if (child != null)
					child.SetActive(false);
			}
		}
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

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		//Persistent data path which is a file path to store game realated data in AppData Roaming  ...
		//.dat  is usually a generic data file that stores information specific to the application it refers to
		//Sometimes you'll find them by themselves but often they're with other configuration files like DLL files
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData();

		data.gold = gold;
		data.vitality = vitality;
		data.strength = strength;
		data.experience = experience;

		bf.Serialize(file, data);

		file.Close();

	}

	//you could also just serialize the class to a string and save it through player prefs
	//keep in mind that this is jsut a way to do it
	public void Load()
	{
		if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();

			gold = data.gold;
			vitality = data.vitality;
			strength = data.strength;
			experience = data.experience;
		}
	}
}


[Serializable] //in a nutshell this makes it so that you can save it to a file
class PlayerData
{
	public int gold;
	public float vitality;
	public float strength;
	public float experience;
	//here you'd want to have getters and setters but for this we're keeping it simple
}
