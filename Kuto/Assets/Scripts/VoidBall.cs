using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBall : MonoBehaviour {
	public float speed = 15f;
	Rigidbody2D rb;
	public GameObject enemy;

	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{	
		if (hitInfo.CompareTag("Player"))
		{
				hitInfo.GetComponent<PlayerHandler>().GetHealthSystem().Damage(34);
				Destroy(gameObject);
		}
	}
}
