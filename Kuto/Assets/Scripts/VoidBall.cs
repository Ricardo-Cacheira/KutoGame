using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBall : MonoBehaviour {
	public float speed = 15f;
	Rigidbody2D rb;
	public GameObject enemy;
	private int dmg = 34;
	private float ranDir;

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
				PlayerHandler player = hitInfo.GetComponent<PlayerHandler>();
				player.GetHealthSystem().Damage(dmg);
				ranDir = Random.Range(1.5f, 4.5f);
				player.CreateText(Color.red, player.transform.position, new Vector2(-1, ranDir), "-" + dmg);
				Destroy(gameObject);
		}
	}
}
