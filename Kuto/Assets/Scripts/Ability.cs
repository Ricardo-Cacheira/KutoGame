using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

	public Transform attackPoint;
	public GameObject bulletPrefab;
	public GameObject player;

	
	void Update () {
		if (Input.GetButtonDown("Fire1"))
		{
			Shoot();
		}
	}

	void Shoot()
	{
		Instantiate(bulletPrefab, player.transform.position, attackPoint.rotation);
	}
}
