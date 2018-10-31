using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameControl : MonoBehaviour {

	public static GameControl control;
	
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
		Load();
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		//Persistent data path which is a file path to store game realated data in AppData Roaming  ...
		//.dat  is usually a generic data file that stores information specific to the application it refers to
		//Sometimes you'll find them by themselves but often they're with other configuration files like DLL files
		// FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		FileStream file = File.Create(Application.persistentDataPath + "playerInfo.json");

		PlayerData data = new PlayerData();

		data.gold = gold;
		data.vitality = vitality;
		data.strength = strength;
		data.experience = experience;
		data.inventoryItems = inventoryItems;
		data.equippedItems = equippedItems;

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
		if(File.Exists(Application.persistentDataPath + "playerInfo.json"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			// FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);			
			// PlayerData data = (PlayerData)bf.Deserialize(file);
			string json = File.ReadAllText(Application.persistentDataPath + "playerInfo.json", System.Text.Encoding.Unicode);
			PlayerData data = JsonUtility.FromJson<PlayerData>(json);
			// file.Close();

			gold = data.gold;
			vitality = data.vitality;
			strength = data.strength;
			experience = data.experience;
			inventoryItems = data.inventoryItems;
			equippedItems = data.equippedItems;
			Debug.Log("Loaded");
			InventoryManager.im.Fill();
		}
		else{
			Debug.Log("Failed to load");
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
