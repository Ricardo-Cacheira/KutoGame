using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBall : MonoBehaviour {
	public float speed = 15f;
	Rigidbody2D rb;
	public GameObject enemy;
	private int dmg;

	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;
		FindObjectOfType<AudioManager>().Play("Woosh");

		dmg = 28 + (GameControl.control.lvl * 2);
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
				player.CreateText(Color.red, new Vector3(player.transform.position.x, player.transform.position.y + 1), new Vector2(0, 5f), "-" + dmg);
				Destroy(gameObject);
		}
	}
}
