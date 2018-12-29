using System.Collections;
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
	public Vector3 moveDir;
    private bool attacked;
    public bool switched;
    private State state;
    Coroutine attack;

    private void Setup(PlayerHandler playerHandler, HealthSystem healthSystem) 
    {
        this.playerHandler = PlayerHandler.playerHandler;
        this.healthSystem = healthSystem;

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private enum State {
        Normal,
        Busy,
    }

    public IEnumerator SwapState(float stunDuration)
    {
        StopCoroutine(attack);
        attacked = false;
        switched = true;
        state = State.Busy;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(.2f, .2f, .2f);
        yield return new WaitForSeconds(stunDuration);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        state = State.Normal;
        switched = false;
    }

    private void FixedUpdate() 
    {  
        if (state == State.Normal) HandleMovement();
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
			attack = StartCoroutine(Attack());
		}
    }

    IEnumerator Attack() 
    {
        attacked = true;
        yield return new WaitForSeconds(.5f);
        Instantiate(GameAssets.i.pfSlowWave, GetPosition(), Quaternion.AngleAxis(Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg, Vector3.forward));
 
        yield return new WaitForSeconds(1f);
        attacked = false;
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
