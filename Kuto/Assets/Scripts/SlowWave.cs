using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowWave : MonoBehaviour {

	public float speed = 10f;
	Rigidbody2D rb;
	public GameObject enemy;

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
				hitInfo.GetComponent<PlayerHandler>().GetHealthSystem().Damage(15);
				hitInfo.GetComponent<PlayerHandler>().SlowPlayer();
				Destroy(gameObject);
		}
	}
}
