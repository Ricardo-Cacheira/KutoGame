using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownTimer : MonoBehaviour {
	public Text cooldownText;
	public float timeLeft;

	public void Start()
	{
		gameObject.SetActive(false);
	}

    void Update()
    {
		// Debug.Log(timeLeft);
    //     timeLeft -= Time.deltaTime;
    //     if(timeLeft  <= 0)
    //     {
	// 		Debug.Log("false");
    //         cooldownText.enabled = false;
	// 	} else 
	// 	{
	// 		Debug.Log("true");
	// 		if (cooldownText.enabled == false) cooldownText.enabled = true;
	// 		cooldownText.text = "" + timeLeft;
	// 	}
    }
}
