using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health;
	public float speed;

	private Animator anim;
	// public GameObject bloodEffect;

	Vector2 dir;

	void Start () 
	{
		// anim.GetComponent<Animator>();
		// anim.SetBool("isRunning", true);
		int randX = Random.Range(-1, 2);
		int randY = Random.Range(-1, 2);
		dir = new Vector2(randX, randY);
	}
	
	void Update () 
	{
		if (health <= 0)
		{
			Destroy(gameObject);
		}
		
		transform.Translate(dir.normalized * speed * Time.deltaTime);
	}

	public void TakeDamage(int damage) 
	{
		//play hurt sound
		// Instantiate(bloodEffect, transform.position, Quaternion.identity);
		health -= damage;
		Debug.Log("Dmg taken");
		Debug.Log("Health: " + health);
	}
}
