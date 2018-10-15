using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	Rigidbody2D rb;
	public GameObject player;

	void Start () {
		GameObject player = GameObject.FindWithTag("Player");
		Movement movScript = player.GetComponent<Movement>();
		Vector2 dir = new Vector2(movScript.lastX,movScript.lastY);
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = dir * speed;
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		Debug.Log(hitInfo.name);
		if(hitInfo.tag != "Player")
		Destroy(gameObject);
	}
}
