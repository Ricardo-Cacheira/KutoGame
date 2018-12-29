using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCollider : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("EnemyRanged")) 
		{
			EnemyRangedHandler enemy = col.GetComponent<EnemyRangedHandler>();
			enemy.GetHealthSystem().Damage(10);
			enemy.KnockBack(1000000);
			if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
			if (enemy.switched == false) StartCoroutine(enemy.SwapState(3));
		}  
		if (col.CompareTag("Enemy"))
		{
			EnemyHandler enemy = col.GetComponent<EnemyHandler>();  
			enemy.GetHealthSystem().Damage(10);
			enemy.KnockBack(1000000);
			if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
			if (enemy.switched == false) StartCoroutine(enemy.SwapState(3));
		}
		if (col.CompareTag("EnemySlower"))
		{
			EnemySlowerHandler enemy = col.GetComponent<EnemySlowerHandler>();  
			enemy.GetHealthSystem().Damage(10);
			enemy.KnockBack(1000000);
			if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
			if (enemy.switched == false) StartCoroutine(enemy.SwapState(3));			
		}
	}
}

