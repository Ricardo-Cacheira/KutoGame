using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	Rigidbody2D rb;
	public GameObject player;

	void Start () 
	{
		player = GameObject.FindWithTag("Player");
		PlayerHandler movScript = player.GetComponent<PlayerHandler>();
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
		if (hitInfo.CompareTag("Enemy"))
		{
			hitInfo.GetComponent<EnemyHandler>().GetHealthSystem().Damage(35);
		} else if (hitInfo.CompareTag("EnemyRanged"))
		{
			hitInfo.GetComponent<EnemyRangedHandler>().GetHealthSystem().Damage(35);
		}
	}	
}
