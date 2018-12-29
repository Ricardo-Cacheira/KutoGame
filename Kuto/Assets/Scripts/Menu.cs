using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MissionType {
    Kill,
	Survive
}

public enum MissionLocation {
	Beach,
	Temple
}

public class Menu : MonoBehaviour {
	public static Menu menu;
	public Sprite beach;
	public Sprite temple;
	public GameObject missionPrefab;
	public GameObject inventory;
	public GameObject missionPanel;
	public GameObject shopPanel;
	public GameObject bossButton;
	bool inventoryVisible;
	bool missionsVisible;
	public bool isBeach, isTemple;
	public GameObject blur;

	void Awake () {
		menu = this;
		inventoryVisible = false;
		missionsVisible = false;
		Time.timeScale = 1;
	}
	
	void Start() {
		if (GameControl.control.xp == GameControl.control.tempXp)
			GameControl.control.isXpMax = true;

		if (GameControl.control.isXpMax)
			bossButton.gameObject.SetActive(true);	
		else 
			bossButton.gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown("f"))
			SceneManager.LoadScene("Prototype");
		if (Input.GetKeyDown("b"))
			SceneManager.LoadScene("BeachScene");
		if (Input.GetKeyDown("m"))
			SceneManager.LoadScene("BossScene");
		if (Input.GetKeyDown("i"))
		{
			// GameControl.control.DropItem();
		}
	}
	
	public void Junk()
	{
		if(inventoryVisible)
		{
			inventory.transform.localPosition = new Vector3(1500f, 0, 0);
			inventoryVisible = false;
			InventoryManager.im.SaveInventory();
			
			shopPanel.SetActive(false);
			blur.SetActive(false);
		}
		else
		{
			inventory.transform.localPosition = new Vector3(0, 0, 0);
			inventoryVisible = true;
			shopPanel.SetActive(true);
			blur.SetActive(true);
		}
	}

	public void Ware()
	{
		if(inventoryVisible)
		{
			inventory.transform.localPosition = new Vector3(1500f, 0, 0);
			inventoryVisible = false;
			InventoryManager.im.SaveInventory();
			shopPanel.SetActive(false);
			blur.SetActive(false);
		}
		else
		{
			inventory.transform.localPosition = new Vector3(0, 0, 0);
			inventoryVisible = true;
			blur.SetActive(true);
			shopPanel.SetActive(false);
		}
	}

	public void Boss()
	{
		SceneManager.LoadScene("BossScene");
	}

	
	public void Phone()
	{
		missionsVisible = !missionsVisible;
		missionPanel.SetActive(missionsVisible);

		if (isBeach && Input.GetButtonDown("Submit")) 
			SceneManager.LoadScene("BeachScene");
		else if (isTemple && Input.GetButtonDown("Submit")) 
			SceneManager.LoadScene("Prototype");

		if(missionsVisible)
		{
			foreach (Transform child in missionPanel.transform) {
				GameObject.Destroy(child.gameObject);
			}
			GenerateMission((MissionType)Random.Range(0, 2),(MissionLocation)Random.Range(0, 2),Random.Range(5, 16));
		}
	}

	public void Load()
	{
		GameControl.control.Load();
	}

	public void Reset()
	{
		GameControl.control.Reset();
	}

	public void Save()
	{
		InventoryManager.im.SaveInventory();
		GameControl.control.Save();
	}

	private void GenerateMission(MissionType missionType, MissionLocation missionLocation, int ammount){
		GameObject newMission = Instantiate(missionPrefab,new Vector3(0,0,0) , Quaternion.identity);
 		newMission.transform.SetParent(missionPanel.transform);
		newMission.transform.localPosition = new Vector3(0, 0, 0);
		newMission.transform.localScale = new Vector3(1, 1, 1);

		string location;

		if(missionLocation == MissionLocation.Beach)
		{
			newMission.transform.Find("Location").GetComponent<Image>().sprite = beach;
			location = "Head down by the beach ";
			isBeach = true;
		}
		else
		{
			newMission.transform.Find("Location").GetComponent<Image>().sprite = temple;
			location = "Travel to the forest temple ";
			isTemple = true;
		}

		if(missionType == MissionType.Kill)
			newMission.transform.Find("Text").GetComponent<Text>().text = location + "and assassinate " + ammount +" targets.";
		else
			newMission.transform.Find("Text").GetComponent<Text>().text = location + "and survive for " + ammount +" minutes.";
		
	}

	public void QuitGame()
	{
		GameControl.control.Save();
		Application.Quit();
	}

}