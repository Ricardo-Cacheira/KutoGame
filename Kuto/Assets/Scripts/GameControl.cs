using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	private Dictionary<int, EquippableItem> itemId = new Dictionary<int, EquippableItem>();

	[SerializeField] List<EquippableItem> equipmentList;
	[Space]
	public List<Item> inventoryItems;
	[Space]
	public List<EquippableItem> equippedItems;
	[Space]

	public int gold;
	public float vitality;
	public float strength;
	public float experience;

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

		for (int i = 0; i < equipmentList.Count; i++)
		{
			itemId.Add(i, equipmentList[i]);
		}
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
		// data.inventoryItems = inventoryItems;
		// data.equippedItems = equippedItems;

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
			inventoryItems = data.inventoryItems;
			equippedItems = data.equippedItems;
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

	public List<Item> inventoryItems;
	public List<EquippableItem> equippedItems;
	//here you'd want to have getters and setters but for this we're keeping it simple
}
