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

	public Sprite beach;
	public Sprite temple;
	public GameObject missionPrefab;
	public GameObject inventory;
	public GameObject missionPanel;
	bool inventoryVisible;
	bool missionsVisible;

	void Awake () {
		inventoryVisible = false;
		missionsVisible = false;
	}
	
	public void Junk()
	{
		Debug.Log(0.1f+0.2f);
		Debug.Log((0.1f+0.2f) == 0.3f);
	}

	public void Ware()
	{
		if(inventoryVisible)
		{
			inventory.transform.localPosition = new Vector3(1500f, 0, 0);
			inventoryVisible = false;
			InventoryManager.im.SaveInventory();
		}
		else
		{
			inventory.transform.localPosition = new Vector3(0, 0, 0);
			inventoryVisible = true;
		}
	}

	public void StartMission()
	{
		SceneManager.LoadScene("Prototype");
	}
	
	public void Phone()
	{
		missionsVisible = !missionsVisible;
		missionPanel.SetActive(missionsVisible);
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

	public void Save()
	{
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
		}
		else
		{
			newMission.transform.Find("Location").GetComponent<Image>().sprite = temple;
			location = "Travel to the forest temple ";
		}

		if(missionType == MissionType.Kill)
			newMission.transform.Find("Text").GetComponent<Text>().text = location + "and assassinate " + ammount +" targets.";
		else
			newMission.transform.Find("Text").GetComponent<Text>().text = location + "and survive for " + ammount +" minutes.";
	}

}