using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	private float timeBtwAttack;
	public float startTimeBtwAttack;

	// public Animator camAnim;
	// public Animator playerAnim;

	public Transform attackPos;
	public LayerMask whatIsEnemies;
	public float attackRange;
	public int damage;
	public GameObject player;
	Movement movScript;

	void Start () {
		movScript = transform.GetComponent<Movement>();
	}

	void Update () 
	{
		if (Input.GetButtonDown("Fire2"))
		{
			
			Attack();	
		}		
	}

	void Attack()
	{
		if (timeBtwAttack <= 0) 
		{
			// camAnim.SetTrigger("shake");
			// playerAnim.SetTrigger("attack");
			Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
			
			for (int i = 0; i < enemiesToDamage.Length; i++)
			{
				enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
			}

			timeBtwAttack = startTimeBtwAttack;
		} else
		{
			timeBtwAttack -= Time.deltaTime;
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPos.position, attackRange);
	}
}
