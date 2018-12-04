using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	public Rigidbody2D rb;
	private int dmg = 35;
	PlayerHandler playerHandler;
	public Vector2 dir;

	void Awake () 
	{
		playerHandler = PlayerHandler.playerHandler.GetComponent<PlayerHandler>();
		dir = new Vector2(playerHandler.lastX, playerHandler.lastY).normalized;
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
		} else if (hitInfo.CompareTag("Boss"))
		{
			BossHandler boss = hitInfo.GetComponent<BossHandler>();
			boss.GetHealthSystem().Damage(dmg);
			CreateText();
		}
	}

	void CreateText()
	{
		playerHandler.CreateText(Color.green, this.gameObject.transform.position, new Vector2(1, 2.5f),"-" + dmg);
	}	
}
