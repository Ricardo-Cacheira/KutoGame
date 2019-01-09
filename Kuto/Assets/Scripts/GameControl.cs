using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	#region Mongo
	// string connectionString = "mongodb://localhost:27017";
	string connectionString = "mongodb://kuto:GAD2019@ds147734.mlab.com:47734/kuto";
	MongoClient client;
	MongoServer server; 
	MongoDatabase database;
	MongoCollection<BsonDocument> playercollection;
	#endregion

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
	public string username;
	public string password;
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
    public bool hasSave;
	public GameObject phone;
	private InputField Iusername;
	private InputField Ipassword;

    void Awake()
	{
		if(control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}else if(control != this)
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		if(GameObject.Find("username") != null)
		{
			Iusername = GameObject.Find("username").GetComponent<InputField>();
			Ipassword = GameObject.Find("password").GetComponent<InputField>();

		
			Iusername.Select();
			Iusername.ActivateInputField();
		}
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
		if (Input.GetKey(KeyCode.Tab) && Ipassword != null) {
			Ipassword.Select();
			Ipassword.ActivateInputField();
		}
		if (Input.GetKey(KeyCode.Return) && Ipassword != null) {
			Login();
		}
		if (Input.GetKey(KeyCode.Tab) && Ipassword != null) {
			Ipassword.Select();
			Ipassword.ActivateInputField();
		}
		if (Input.GetKey(KeyCode.Return) && Ipassword != null) {
			Login();
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		Connect();
        // Debug.Log("OnSceneLoaded: " + scene.name);
        // Debug.Log(mode);

		if(scene.name == "town")
		{
			if(!loaded)
			{
				Load();
				CalculateLevel();
			}
		}
    }

	public void Connect()
	{
		client = new MongoClient(connectionString);
		server = client.GetServer(); 
		database = server.GetDatabase("kuto");
		playercollection= database.GetCollection<BsonDocument>("accounts");
		Debug.Log ("1. ESTABLISHED CONNECTION");	
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
		int itemLevel = UnityEngine.Random.Range(lvl-10, lvl+2);
		if(itemLevel <= 0)
			item.level = 1;
		else
			item.level = itemLevel;
		
		inventoryItems.Add(item);

		return item;
	}

	public void Login()
	{
		username = Iusername.text;
		password = Ipassword.text;

		if (username.Length < 3)
		{
			ErrorMsg("Username must have at least 4 characters");
			return;
		}

		var user = "";
		var pass = "";
		var query = new QueryDocument("username", username);
		foreach (var document in playercollection.Find(query)) {			
			user = document["username"].ToString();
			pass = document["password"].ToString();
		}

		if(username == user && password == pass)
			SceneManager.LoadScene ("town");
		else
			ErrorMsg("Invalid username and/or password!");

	}

	private void ErrorMsg(string text)
    { 
        GameObject canvasObj = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform tempTextBox = Instantiate(GameAssets.i.combatText, new Vector3(Screen.width/2, Screen.height/2 + 50), transform.rotation);
        tempTextBox.transform.SetParent(canvasObj.transform, true);
        tempTextBox.GetComponent<Text>().color = Color.red;
        tempTextBox.GetComponent<CombatText>().Initialize(2, text, new Vector3(0, 25));
    }

	public void Register()
	{
		
		username = Iusername.text;
		password = Ipassword.text;

		if (username.Length < 3)
		{
			ErrorMsg("Username must have at least 4 characters");
			return;
		}
		var query = Query.EQ("username", username);
		BsonDocument result = playercollection.FindOne(query);
		if (result != null)
		{
			ErrorMsg("Username already taken!");
			return;
		}

		PlayerData data = new PlayerData();

		data.gold = 150;
		data.shards = 5;
		data.vitality = 100;
		data.strength = 20;
		data.xp = 0;

		var items = itemDatabase.GetAll();

		foreach (var item in items)
		{
			ItemData itemData = new ItemData(item.ID, item.level);
			data.inventoryItems.Add(itemData);
		}

		data.equippedItems = new List<ItemData>(); 

		var bsonDocument = data.ToBsonDocument();

		playercollection.Insert(new BsonDocument{
			{ "username", username },
			{ "password", password },
			{ "new",  true},
			{ "data",  bsonDocument}
		});
		
		SceneManager.LoadScene ("town");
	}

	public void Save() {
		//Sometimes you'll find them by themselves but often they're with other configuration files like DLL files
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.json");

		PlayerData data = new PlayerData();

		data.username = username;
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
		
		var bsonDocument = data.ToBsonDocument();

		var query = Query.EQ("username", username);
		BsonDocument result = playercollection.FindOne(query);
		if (result != null)
		{
			result["data"] = bsonDocument;
			playercollection.Save(result);
			// playercollection.Remove(Query.EQ("username", username));		
		}else
			playercollection.Insert(bsonDocument);			
		
		file.Close();
	}

	//you could also just serialize the class to a string and save it through player prefs
	//keep in mind that this is jsut a way to do it
	public void Load()
	{
		GameObject tut = GameObject.Find("Tutorial");
		loaded = false;

		PlayerData data = new PlayerData();
		
		bool isNew = false;
		var query = new QueryDocument("username", username);
		foreach (var document in playercollection.Find(query)) {			
			username = document["username"].ToString();
			isNew = document["new"].ToBoolean();
			data = BsonSerializer.Deserialize<PlayerData>(document["data"].ToBson());
		}
		
		if(!isNew)
		{
			string json;
			if(File.Exists(Application.persistentDataPath + "/playerInfo.json"))
				json = File.ReadAllText(Application.persistentDataPath + "/playerInfo.json", System.Text.Encoding.Unicode);
			

			// if(data != null)
			// data = JsonUtility.FromJson<PlayerData>(json);
			
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
			if(sceneName == "town") {
				InventoryManager.im.Fill();
				tut.GetComponent<Image>().enabled = false;
				tut.GetComponent<Button>().enabled = false;
			}
			
			if (InventoryManager.im.inventory != null) InventoryManager.im.inventory.RefreshUI();
			hasSave = true;
		} else {
			Debug.Log("New");
			hasSave = false;
			tut.GetComponent<Image>().enabled = true;
			tut.GetComponent<Button>().enabled = true;
			phone = GameObject.Find("Phone");
			phone.SetActive(false);
			BsonDocument result = playercollection.FindOne(query);
			if (result != null)
			{
				result["new"] = false;
				playercollection.Save(result);
				// playercollection.Remove(Query.EQ("username", username));		
			}
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


[BsonIgnoreExtraElements][Serializable] //in a nutshell this makes it so that you can save it to a file
class PlayerData
{
	public string username;
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