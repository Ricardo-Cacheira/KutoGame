using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class FireBallBoss : MonoBehaviour {

	public float speed = 18f;
	private PlayerHandler playerHandler;
	private Vector2 dir;
	private float randX, randY;
	private Rigidbody2D rb2d;
	
	void Start () 
	{
		randX = Random.Range(-1f, 1f);
		randY = Random.Range(-1f, 1f);
		dir = new Vector2(randX, randY).normalized;
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		rb2d.velocity = dir * speed;
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		if (hitInfo.CompareTag("Player"))
		{
			PlayerHandler.playerHandler.GetHealthSystem().Damage(BossHandler.dmgShooting);
			PlayerHandler.playerHandler.KnockBack(1500, this.transform.position);

			Destroy(gameObject);
			CreateText();
		} 
	}

	void CreateText()
	{
		PlayerHandler.playerHandler.CreateText(Color.red, this.gameObject.transform.position, new Vector2(0, 5f),"-" + BossHandler.dmgShooting);
	}
}
