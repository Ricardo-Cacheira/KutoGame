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
    private int dmg;
    private float ranDir;

    Animator animator;


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
        animator = gameObject.GetComponent<Animator>();
        attackPoint.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        dmg = 15;
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
        animator.SetTrigger("Charging");
        StartCoroutine(ChargeAttack(1, 1f, attackPoint.GetComponent<SpriteRenderer>().color));
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("Attack");
        attackPoint.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        SetStateNormal();
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, 0.7f, whatIsPlayer);
        if (player != null) {
            PlayerHandler.playerHandler.GetHealthSystem().Damage(dmg);
            ranDir = UnityEngine.Random.Range(1.5f, 4.5f);
            playerHandler.CreateText(Color.red, playerHandler.transform.position, new Vector2(-1, ranDir), "-" + dmg);
        }
        
        if (PlayerHandler.playerHandler.GetHealthSystem().GetHealthPercent() <= 0) {
            GameHandler.Restart();
        }
        
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator ChargeAttack(float aValue, float aTime, Color cooldownColor)
    {
        float alpha = cooldownColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, aValue,t));
            attackPoint.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }
    }

    public void KnockBack(float force)
    {   
        Vector2 knock = (this.transform.position - playerHandler.transform.position).normalized;
        gameObject.GetComponent<Rigidbody2D>().AddForce(knock * force);
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, 0.7f);
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
        if (OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}
