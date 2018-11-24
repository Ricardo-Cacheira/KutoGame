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
}
