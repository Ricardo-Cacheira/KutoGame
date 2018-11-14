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

	bool missionSelected;

	void Update () 
	{	
		if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
		{	
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;

		} 
		if (selectedObject.name == "Phone" && !missionSelected) {
			selectedObject.GetComponent<Button>().onClick.AddListener(MissionBool);			
		} else if (selectedObject.name == "Phone" && missionSelected) {
			selectedObject.GetComponent<Button>().onClick.AddListener(MissionStart);

		}

	}

	void MissionBool()
	{
		missionSelected = true;
	}

	void MissionStart()
	{
		GameObject menu = GameObject.Find("Canvas");
		Menu menuspt = menu.GetComponent<Menu>();
		menuspt.StartMission();
	}

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
