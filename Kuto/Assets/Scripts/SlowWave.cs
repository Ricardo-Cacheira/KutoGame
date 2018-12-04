using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowWave : MonoBehaviour {

	public float speed = 10f;
	Rigidbody2D rb;
	public GameObject enemy;
	private int dmg = 10;
	private float ranDir;

	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = transform.right * speed;
	}

	void Update()
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
				ranDir = Random.Range(1.5f, 4.5f);
				player.CreateText(Color.red, player.transform.position, new Vector2(-1, ranDir),"-" + dmg);
				Destroy(gameObject);
		}
	}
}
