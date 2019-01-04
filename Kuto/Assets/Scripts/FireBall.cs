using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	public float speed = 20f;
	public Rigidbody2D rb;
	private int dmg;
	PlayerHandler playerHandler;
	public Vector2 dir;

	void Awake () 
	{
		playerHandler = PlayerHandler.playerHandler.GetComponent<PlayerHandler>();
		dir = new Vector2(playerHandler.lastX, playerHandler.lastY).normalized;
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = dir * speed;
		dmg = playerHandler.shootingDmg;
		Destroy(gameObject, 2);
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
			CreateText(hitInfo.transform.position);
		} else if (hitInfo.CompareTag("EnemyRanged"))
		{
			EnemyRangedHandler enemy = hitInfo.GetComponent<EnemyRangedHandler>();
			enemy.GetHealthSystem().Damage(dmg);
			enemy.KnockBack(400000);
			CreateText(hitInfo.transform.position);		
		} else if (hitInfo.CompareTag("EnemySlower"))
		{
			EnemySlowerHandler enemy = hitInfo.GetComponent<EnemySlowerHandler>();
			enemy.GetHealthSystem().Damage(dmg);
			enemy.KnockBack(400000);
			CreateText(hitInfo.transform.position);
		} else if (hitInfo.CompareTag("Boss"))
		{
			BossHandler boss = hitInfo.GetComponent<BossHandler>();
			boss.GetHealthSystem().Damage(dmg);
			CreateText(hitInfo.transform.position);
		}
	}

	void CreateText(Vector3 pos)
	{
		playerHandler.CreateText(Color.gray, pos, new Vector2(0, 1f),"-" + dmg);
	}	
}
