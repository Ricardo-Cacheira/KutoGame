using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health;
	public float speed;

	private Animator anim;
	// public GameObject bloodEffect;

	void Start () 
	{
		// anim.GetComponent<Animator>();
		// anim.SetBool("isRunning", true);
	}
	
	void Update () 
	{
		if (health <= 0)
		{
			Destroy(gameObject);
		}

		transform.Translate(Vector2.left * speed * Time.deltaTime);
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
