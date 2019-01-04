using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyRangedHandler : MonoBehaviour {

	public static EnemyRangedHandler CreateEnemy(Vector3 spawnPosition, PlayerHandler playerHandler) 
    {
        Transform enemyTransform = Instantiate(GameAssets.i.pfEnemyRangedTransform, spawnPosition, Quaternion.identity);
        EnemyRangedHandler enemyRangedHandler = enemyTransform.GetComponent<EnemyRangedHandler>();

        HealthSystem healthSystem = new HealthSystem(75 + (GameControl.control.lvl * 9));
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 1.5f), Quaternion.identity, enemyTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        enemyRangedHandler.Setup(playerHandler, healthSystem);
        

        return enemyRangedHandler;
    }

    public event EventHandler OnDead;

    private const float speed = 5f;

    private PlayerHandler playerHandler;
    private HealthSystem healthSystem;
	public Vector3 moveDir;
    private State state;
    public bool switched;
    private Coroutine attack;

    private enum State {
        Normal,
        Busy,
    }

    public IEnumerator SwapState(float stunDuration)
    {
        switched = true;
        if (attack != null) StopCoroutine(attack); 
        state = State.Busy;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(.2f, .2f, .2f);
        yield return new WaitForSeconds(stunDuration);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        state = State.Normal;
        switched = false;
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
            }
            break;

        case State.Busy:
            break;
        }
    }

    private void HandleMovement() 
    {	
		float distanceToPlayer = Vector3.Distance(PlayerHandler.playerHandler.GetPosition(), GetPosition());
		moveDir = (PlayerHandler.playerHandler.GetPosition() - GetPosition()).normalized;

		if (distanceToPlayer > 8) 
        	transform.position = transform.position + moveDir * speed * Time.deltaTime;
		else if (distanceToPlayer <= 6)
			transform.position = transform.position + (-moveDir) * (speed/2) * Time.deltaTime;
		else 
		{
			SetStateBusy();
			attack = StartCoroutine(Attack());
		}
    }

    IEnumerator Attack() 
    {
        yield return new WaitForSeconds(1.5f);
        SetStateNormal();
        Instantiate(GameAssets.i.pfVoidBall, GetPosition(), Quaternion.AngleAxis(Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg, Vector3.forward));
    }

    public void KnockBack(float force)
    {   
        Vector2 knock = (this.transform.position - playerHandler.transform.position).normalized;
        gameObject.GetComponent<Rigidbody2D>().AddForce(knock * force);
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
