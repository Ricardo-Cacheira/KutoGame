using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyRangedHandler : MonoBehaviour {

	public static EnemyRangedHandler CreateEnemy(Vector3 spawnPosition, PlayerHandler playerHandler) 
    {
        Transform enemyTransform = Instantiate(GameAssets.i.pfEnemyRangedTransform, spawnPosition, Quaternion.identity);
        EnemyRangedHandler enemyRangedHandler = enemyTransform.GetComponent<EnemyRangedHandler>();

        HealthSystem healthSystem = new HealthSystem(75);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 1.5f), Quaternion.identity, enemyTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        enemyRangedHandler.Setup(playerHandler, healthSystem);
        

        return enemyRangedHandler;
    }

    public event EventHandler OnDead;

    private const float speed = 5f;

    private PlayerHandler playerHandler;
    private HealthSystem healthSystem;
    private Vector3 lastMoveDir;
	private Vector3 moveDir;
    private State state;
	public Transform attackPoint;
	public LayerMask whatIsPlayer;

    private enum State {
        Normal,
        Busy,
    }

    private void Setup(PlayerHandler playerHandler, HealthSystem healthSystem) 
    {
        this.playerHandler = PlayerHandler.playerHandler;
        this.healthSystem = healthSystem;

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void Start() 
    {
        state = State.Normal;
        
    }

    private void Update() 
    {  
        switch (state) 
        {
        case State.Normal:
            if (PlayerHandler.playerHandler.IsDead()) 
            {} 
            else 
            {
                HandleMovement();
                HandleAttack();
            }
            break;

        case State.Busy:
            break;
        }
    }

    private void HandleMovement() 
    {	
		float distanceToPlayer = Vector3.Distance(PlayerHandler.playerHandler.GetPosition(), GetPosition());
		Vector3 moveDir = (PlayerHandler.playerHandler.GetPosition() - GetPosition()).normalized;

		if (distanceToPlayer > 8) 
		{
        	transform.position = transform.position + moveDir * speed * Time.deltaTime;
		} else if (distanceToPlayer <= 6) {
			transform.position = transform.position + (-moveDir) * (speed/2) * Time.deltaTime;
		} else 
		{
			// Instantiate(GameAssets.i.pfFireBall, GetPosition(), attackPoint.rotation);
			// SetStateBusy();
			// StartCoroutine(Attack());
		}
    }

	// private void HandleAttack() 
    // {
    //     float distanceToPlayer = Vector3.Distance(GetPosition(), playerHandler.GetPosition());
    //     if (distanceToPlayer < 1.5f) 
    //     {  
    //         attackPoint.position = transform.position + (Vector3) moveDir;

    //         if (state == State.Normal) 
    //         {
    //             SetStateBusy();
    //             StartCoroutine(Attack());
    //         } 
    //     }
    // }

    IEnumerator Attack() 
    {
        yield return new WaitForSeconds(1);
        SetStateNormal();
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, 1, whatIsPlayer);
        player.GetComponent<PlayerHandler>().GetHealthSystem().Damage(10);
        if (player.GetComponent<PlayerHandler>().GetHealthSystem().GetHealthPercent() <= 0) {
            GameHandler.Restart();
        }
    }

    private void HandleAttack() 
    {
        float distanceToPlayer = Vector3.Distance(GetPosition(), PlayerHandler.playerHandler.GetPosition());
    }

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(moveDir, 1);
	}

    private void SetStateBusy() 
    {
        state = State.Busy;
    }

    private void SetStateNormal() 
    {
        state = State.Normal;
    }

    public Vector3 GetPosition() 
    {
        return transform.position;
    }

    public HealthSystem GetHealthSystem() 
    {
        return healthSystem;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) 
    {
        // Dead! Destroy self
        if (OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}
