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
	[Space]
	public List<Ability> abilities = new List<Ability>();
	[Space]
	public List<Item> inventoryItems = new List<Item>();
	[Space]
	public List<EquippableItem> equippedItems = new List<EquippableItem>();
	[Space]
	public int[] cooldowns = new int[3];
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
	public bool loaded;

	void Awake()
	{
		if(control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
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

	void Update()
	{
		if (Input.GetKeyDown("v")) {
			xp = 118000;
			CalculateLevel();
		}
		if (Input.GetKeyDown("c")) {
			xp = 1332000;
			CalculateLevel();
		}
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
			currReqXp = 120 + (i * i * 10);
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

	public void SaveCooldowns()
	{
		int i = 0;
		foreach (Transform child in InventoryManager.im.cooldownParent.transform)
		{
			cooldowns[i] = child.GetComponent<Cooldown>().skill;
			i++;
		}
	}
	
	public Item DropItem()
	{
		Item item = itemDatabase.GetItemCopy(itemDatabase.RandomID());
		item.level = 1;
		inventoryItems.Add(item);

		return item;
		// InventoryManager.im.Fill();
			
		// InventoryManager.im.inventory.RefreshUI();
	}

	public void Save() {
		//Sometimes you'll find them by themselves but often they're with other configuration files like DLL files
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.json");

		PlayerData data = new PlayerData();

		data.gold = gold;
		data.shards = shards;
		data.vitality = vitality;
		data.strength = strength;
		data.xp = xp;

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

		file.Close();
	}

	//you could also just serialize the class to a string and save it through player prefs
	//keep in mind that this is jsut a way to do it
	public void Load()
	{
		loaded = false;
		if(File.Exists(Application.persistentDataPath + "/playerInfo.json"))
		{
			string json = File.ReadAllText(Application.persistentDataPath + "/playerInfo.json", System.Text.Encoding.Unicode);
			PlayerData data = JsonUtility.FromJson<PlayerData>(json);

			gold = data.gold;
			shards = data.shards;
			vitality = data.vitality;
			strength = data.strength;
			xp = data.xp;

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

			Scene m_Scene;
			string sceneName;
			m_Scene = SceneManager.GetActiveScene();
			sceneName = m_Scene.name;
			if(sceneName == "town")
				InventoryManager.im.Fill();
			
			if (InventoryManager.im.inventory != null) InventoryManager.im.inventory.RefreshUI();

		}
		else{
			Debug.Log("Failed to load");
		}
		loaded = true;
	}

    public void Reset()
    {
		Scene loadedLevel = SceneManager.GetActiveScene ();
		if(File.Exists(Application.persistentDataPath + "/playerInfo.json"))
		{
			File.Delete(Application.persistentDataPath + "/playerInfo.json");
     		SceneManager.LoadScene (loadedLevel.buildIndex);
		}
		Save();
		SceneManager.LoadScene (loadedLevel.buildIndex);
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