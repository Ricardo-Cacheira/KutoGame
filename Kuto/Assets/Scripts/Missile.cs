using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    private Rigidbody2D rb2d;
    GameObject player;
    private int missileDmg;
    void Start() 
    {
        rb2d = GetComponent<Rigidbody2D>();
        missileDmg = 25 + (GameControl.control.lvl * 2);
    }

    void FixedUpdate()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < 5) 
        {
            rb2d.velocity = transform.right * 17f;
        } else 
        {
            Vector3 moveDir = (player.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) {
            col.GetComponent<PlayerHandler>().GetHealthSystem().Damage(50);
            PlayerHandler.playerHandler.CreateText(Color.red, transform.position, new Vector2(-1, 3.5f), "-" + 50);

            Destroy(gameObject);
        }

        if (col.CompareTag("Walls")) {
            Destroy(gameObject);
        }
    }

}
