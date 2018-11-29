using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	Rigidbody2D rb;
	public GameObject player;
	private int dmg = 35;
	PlayerHandler playerHandler;

	void Start () 
	{
		player = GameObject.FindWithTag("Player");
		playerHandler = player.GetComponent<PlayerHandler>();
		Vector2 dir = new Vector2(playerHandler.lastX, playerHandler.lastY).normalized;
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
			EnemyHandler enemy = hitInfo.GetComponent<EnemyHandler>();
			enemy.GetHealthSystem().Damage(dmg);
			enemy.KnockBack(400000);
			CreateText();
		} else if (hitInfo.CompareTag("EnemyRanged"))
		{
			EnemyRangedHandler enemy = hitInfo.GetComponent<EnemyRangedHandler>();
			enemy.GetHealthSystem().Damage(dmg);
			enemy.KnockBack(400000);
			CreateText();		
		} else if (hitInfo.CompareTag("EnemySlower"))
		{
			EnemySlowerHandler enemy = hitInfo.GetComponent<EnemySlowerHandler>();
			enemy.GetHealthSystem().Damage(dmg);
			enemy.KnockBack(400000);
			CreateText();
		} else if (hitInfo.CompareTag("Walls"))
		{
			Destroy(gameObject);
		}
	}

	void CreateText()
	{
		playerHandler.CreateText(Color.green, this.gameObject.transform.position, new Vector2(1, 2.5f),"-" + dmg);
	}	
}
