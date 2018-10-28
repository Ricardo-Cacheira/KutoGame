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
		Debug.Log("Junk");
	}

	public void Ware()
	{
		// bool inventoryActive = inventory.activeSelf;

		// if (inventory != null)
		// 	inventory.SetActive(!inventoryActive);
		
		Debug.Log(inventoryVisible);

		if(inventoryVisible)
		{
			inventory.transform.Translate(1500,0,0);
			inventoryVisible = false;
			InventoryManager.im.SaveInventory();
		}
		else
		{
			inventory.transform.Translate(-1500,0,0);
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