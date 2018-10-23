using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

	public static GameControl control;
	//to access properties in here use
	//GameControl.control.VARIABLE
	//this works because teh class has this static reference 

	public int health;
	public float experience;

	void Awake () {
		if(control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
		}else if(control != this)
		{
			//if one exists and its not THIS destroy THIS so the other one is the only one
			Destroy(gameObject);
		}
	}
	
	void Update () {
		
	}
}
