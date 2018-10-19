using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	Rigidbody2D rb;
	public GameObject player;

	void Start () {
		player = GameObject.FindWithTag("Player");
		Movement movScript = player.GetComponent<Movement>();
		Vector2 dir = new Vector2(movScript.lastX,movScript.lastY).normalized;
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = dir * speed;
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		Debug.Log(hitInfo.name);
		if(hitInfo.tag != "Player") 
		{
			if (hitInfo.tag == "Enemy")
			{
				hitInfo.GetComponent<Enemy>().TakeDamage(1);

			}
		Destroy(gameObject);
		}
	}
}
