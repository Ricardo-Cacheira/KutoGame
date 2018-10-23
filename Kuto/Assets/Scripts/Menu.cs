using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	public GameObject inventory;

	void Start () {
		inventory.SetActive(false);
	}
	
	public void Junk()
	{
		Debug.Log("Junk");
	}

	public void Ware()
	{
		bool inventoryActive = inventory.activeSelf;

		if (inventory != null)
			inventory.SetActive(!inventoryActive);
	}
	
	public void Phone()
	{
		Debug.Log("phone");
	}
}
