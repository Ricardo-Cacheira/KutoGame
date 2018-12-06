using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;
	public Menu menu;

	private bool buttonSelected;

	void Update () 
	{	
		if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
		{	
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		} 
	}

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
