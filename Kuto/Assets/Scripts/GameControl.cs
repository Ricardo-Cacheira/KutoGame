using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {

	public static GameControl control;
	
	[SerializeField] ItemDatabase itemDatabase;
	// [SerializeField] ItemSaveManager itemSaveManager;
	
	public List<Item> inventoryItems = new List<Item>();
	[Space]
	public List<EquippableItem> equippedItems = new List<EquippableItem>();
	[Space]

	public int gold;
	public int shards;
	public float vitality = 100;
	public float strength;
	public int xp;
	public int lvl;
	public int currReqXp;
	public int lastReqXp;
	public int tempXp;
	public bool isXpMax;

	void Awake()
	{
		if(control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
			Debug.Log("calculated");
		}else if(control != this)
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		Load();
		CalculateLevel();
	}


	public int GetLevel()
	{
		int number = (int)(xp / 1000) + 1;

		return number;
	}

	public void CalculateLevel()
	{
		currReqXp = 0;
		tempXp = 0;
		lastReqXp = 0;

		for (int i = 1; i < 999; i++)
		{
			currReqXp = 200 + (i * i * 10);
			tempXp += currReqXp;

			if (tempXp >= xp) 
			{
				lvl = i;
				if (i == 1) lastReqXp = 0;
				break;
			} 
			else 
				lastReqXp += currReqXp;
		}
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		//Persistent data path which is a file path to store game realated data in AppData Roaming  ...
		//.dat  is usually a generic data file that stores information specific to the application it refers to
		//Sometimes you'll find them by themselves but often they're with other configuration files like DLL files
		// FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.json");

		PlayerData data = new PlayerData();

		data.gold = gold;
		data.shards = shards;
		data.vitality = vitality;
		data.strength = strength;
		data.xp = xp;
		// data.inventoryItems = inventoryItems;
		// data.equippedItems = equippedItems;


		foreach (var item in inventoryItems)
		{
			ItemData itemData = new ItemData(item.ID, item.level);
			data.inventoryItems.Add(itemData);
		}

		foreach (var item in equippedItems)
		{
			ItemData itemData = new ItemData(item.ID, item.level);
			data.equippedItems.Add(itemData);
		}

		string json = JsonUtility.ToJson(data, true);
		byte[] bytes = System.Text.Encoding.Unicode.GetBytes(json);
		file.Write(bytes, 0, bytes.Length);
		// bf.Serialize(file, json);

		file.Close();

		Debug.Log("Saved");

	}

	//you could also just serialize the class to a string and save it through player prefs
	//keep in mind that this is jsut a way to do it
	public void Load()
	{
		if(File.Exists(Application.persistentDataPath + "/playerInfo.json"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			// FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);			
			// PlayerData data = (PlayerData)bf.Deserialize(file);
			string json = File.ReadAllText(Application.persistentDataPath + "/playerInfo.json", System.Text.Encoding.Unicode);
			PlayerData data = JsonUtility.FromJson<PlayerData>(json);
			// file.Close();

			gold = data.gold;
			shards = data.shards;
			vitality = data.vitality;
			strength = data.strength;
			xp = data.xp;
			// inventoryItems = data.inventoryItems;
			// equippedItems = data.equippedItems;

			inventoryItems.Clear();
			equippedItems.Clear();
			foreach (var itemData in data.inventoryItems)
			{
				Item item = itemDatabase.GetItemCopy(itemData.id);
				item.level = itemData.level;
				inventoryItems.Add(item);
			}

			foreach (var itemData in data.equippedItems)
			{
				EquippableItem item = (EquippableItem)itemDatabase.GetItemCopy(itemData.id);
				item.level = itemData.level;
				equippedItems.Add(item);
			}



			Debug.Log("Loaded");

			Scene m_Scene;
			string sceneName;
			m_Scene = SceneManager.GetActiveScene();
			sceneName = m_Scene.name;
			if(sceneName == "town")
				InventoryManager.im.Fill();
			
			InventoryManager.im.inventory.RefreshUI();
		}
		else{
			Debug.Log("Failed to load");
		}

		Debug.Log(Application.persistentDataPath);
	}

}


[Serializable] //in a nutshell this makes it so that you can save it to a file
class PlayerData
{
	public int gold;
	public int shards;
	public float vitality;
	public float strength;
	public int xp;

	public List<ItemData> inventoryItems = new List<ItemData>();
	public List<ItemData> equippedItems = new List<ItemData>();

	// public List<Item> inventoryItems;
	// public List<EquippableItem> equippedItems;
	//here you'd want to have getters and setters but for this we're keeping it simple
}

[Serializable]
class ItemData
{
	public string id;
	public int level;

    public ItemData(string id, int level)
    {
		this.id = id;
		this.level = level;
    }
}