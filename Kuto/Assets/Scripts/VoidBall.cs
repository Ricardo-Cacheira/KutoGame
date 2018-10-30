using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBall : MonoBehaviour {
	public float speed = 15f;
	Rigidbody2D rb;
	public GameObject enemy;

	void Start () 
	{
		enemy = GameObject.FindWithTag("EnemyRanged");
		//EnemyRangedHandler movScript = enemy.GetComponent<EnemyRangedHandler>();
		//Vector2 dir = new Vector2(movScript.moveDir.x,movScript.moveDir.y).normalized;
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		if(hitInfo.tag != "EnemyRanged") 
		{
			if (hitInfo.tag == "Player")
			{
				hitInfo.GetComponent<PlayerHandler>().GetHealthSystem().Damage(34);
			}
		
		Destroy(gameObject);
		}
	}
}
