﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySlowerHandler : MonoBehaviour {

	public static EnemySlowerHandler CreateEnemy(Vector3 spawnPosition, PlayerHandler playerHandler) 
    {
        Transform enemyTransform = Instantiate(GameAssets.i.pfEnemySlowerTransform, spawnPosition, Quaternion.identity);
        EnemySlowerHandler enemySlowerHandler = enemyTransform.GetComponent<EnemySlowerHandler>();

        HealthSystem healthSystem = new HealthSystem(115);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 1.5f), Quaternion.identity, enemyTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        enemySlowerHandler.Setup(playerHandler, healthSystem);
        

        return enemySlowerHandler;
    }

    public event EventHandler OnDead;

    private const float speed = 6.5f;

    private PlayerHandler playerHandler;
    private HealthSystem healthSystem;
    private Vector3 lastMoveDir;
	public Vector3 moveDir;
    private State state;
	public Transform attackPoint;
	public LayerMask whatIsPlayer;

    private bool attacked;

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
		float distanceToPlayer = Vector3.Distance(PlayerHandler.playerHandler.GetPosition(), GetPosition());
		moveDir = (PlayerHandler.playerHandler.GetPosition() - GetPosition()).normalized;

		if (distanceToPlayer > 8) 
		{
        	transform.position = transform.position + moveDir * speed * Time.deltaTime;
		} else if (distanceToPlayer <= 4) {
			transform.position = transform.position + (-moveDir) * (speed/2) * Time.deltaTime;
		} else if (!attacked)
		{
			SetStateBusy();
			StartCoroutine(Attack());
		}
    }

    IEnumerator Attack() 
    {
        attacked = true;
        yield return new WaitForSeconds(.5f);
        SetStateNormal();
        Instantiate(GameAssets.i.pfSlowWave, GetPosition(), Quaternion.AngleAxis(Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg, Vector3.forward));
        if (PlayerHandler.playerHandler.GetHealthSystem().GetHealthPercent() <= 0) {
            GameHandler.Restart();
        }
        yield return new WaitForSeconds(1f);
        attacked = false;
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