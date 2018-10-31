using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public GameObject inventory;
	bool inventoryVisible;

	void Awake () {
		inventoryVisible = false;
		// inventory.SetActive(false);
	}
	
	public void Junk()
	{
		Debug.Log(0.1f+0.2f);
		Debug.Log((0.1f+0.2f) == 0.3f);
	}

	public void Ware()
	{
		// bool inventoryActive = inventory.activeSelf;

		// if (inventory != null)
		// 	inventory.SetActive(!inventoryActive);
		
		Debug.Log(inventoryVisible);

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
	
	public void Phone()
	{
		SceneManager.LoadScene("Prototype");
	}

	public void Load()
	{
		GameControl.control.Load();
	}

	public void Save()
	{
		GameControl.control.Save();
	}
}