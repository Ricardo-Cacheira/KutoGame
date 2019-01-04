using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowWave : MonoBehaviour {

	public float speed = 10f;
	Rigidbody2D rb;
	public GameObject enemy;
	private int dmg;

	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;

		dmg = 10 + (GameControl.control.lvl);
	}

	void FixedUpdate()
	{
		this.transform.localScale += new Vector3(.1f, .1f, .1f);
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
				player.SlowPlayer();
				player.CreateText(Color.red, new Vector3(player.transform.position.x, player.transform.position.y + 1), new Vector2(0, 5f),"-" + dmg);
				Destroy(gameObject);
		}
	}
}
