using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHandler : MonoBehaviour {

	public static EnemyHandler CreateEnemy(Vector3 spawnPosition, PlayerHandler playerHandler) 
    {
        Transform enemyTransform = Instantiate(GameAssets.i.pfEnemyTransform, spawnPosition, Quaternion.identity);
        EnemyHandler enemyHandler = enemyTransform.GetComponent<EnemyHandler>();

        HealthSystem healthSystem = new HealthSystem(200);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 1.5f), Quaternion.identity, enemyTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        enemyHandler.Setup(playerHandler, healthSystem);

        return enemyHandler;
    }

    public event EventHandler OnDead;

    private const float speed = 3f;

    private PlayerHandler playerHandler;
    private HealthSystem healthSystem;
    private Vector3 lastMoveDir;
    private State state;

    public Transform attackPoint;
    bool attacking;
    private Vector3 moveDir;
    public LayerMask whatIsPlayer;


    private enum State {
        Normal,
        Busy,
    }

    private void Setup(PlayerHandler playerHandler, HealthSystem healthSystem) 
    {
        this.playerHandler = playerHandler;
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
                HandleMovement();
                HandleAttack();
            break;
        case State.Busy:
            HandleAttack();
            break;
        }
    }

    private void HandleMovement() 
    {
        moveDir = (playerHandler.GetPosition() - GetPosition()).normalized;
        transform.position = transform.position + moveDir * speed * Time.deltaTime;
    }

    private void HandleAttack() 
    {
        float distanceToPlayer = Vector3.Distance(GetPosition(), playerHandler.GetPosition());
        if (distanceToPlayer < 1.5f) 
        {  
            attackPoint.position = transform.position + (Vector3) moveDir;

            if (state == State.Normal) 
            {
                SetStateBusy();
                StartCoroutine(Attack());
            } 
        }
    }

    IEnumerator Attack() 
    {
        yield return new WaitForSeconds(1);
        SetStateNormal();
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, 1, whatIsPlayer);
        PlayerHandler.playerHandler.GetHealthSystem().Damage(10);
        if (PlayerHandler.playerHandler.GetHealthSystem().GetHealthPercent() <= 0) {
            GameHandler.Restart();
        }
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, 1);
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
