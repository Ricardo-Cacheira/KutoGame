using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;

	private bool buttonSelected;

	void Update () 
	{	
		if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
		{
			eventSystem.SetSelectedGameObject(null);
	
			eventSystem.SetSelectedGameObject(selectedObject);
			Debug.Log(selectedObject);
			buttonSelected = true;
		}
	}

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
