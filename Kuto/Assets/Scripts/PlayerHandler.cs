using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler CreatePlayer(Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc) 
    {
        Transform playerTransform = Instantiate(GameAssets.i.pfPlayerTransform, new Vector3(0, 0), Quaternion.identity);
        
        HealthSystem healthSystem = new HealthSystem(100);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, new Vector3(0, 1.5f), Quaternion.identity, playerTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        PlayerHandler playerHandler = playerTransform.GetComponent<PlayerHandler>();
        playerHandler.Setup(healthSystem, getClosestEnemyHandlerFunc);

        return playerHandler;
    }
    public static PlayerHandler playerHandler;

	public event EventHandler OnDead;
    public Transform attackPoint;

    public Rigidbody2D rigidbody2D;
	public float attackRange;
	public int damage;
    public LayerMask whatIsEnemies;

    private HealthSystem healthSystem;
    private Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc;
    private Vector3 lastMoveDir;
    private State state;

	public Vector2 movement;

	bool dashing, leaping;
	public float speed = 6f;
	public float dashSpeed = 10f;
	public float dashTime = 0.3f;
	public float recoveryTime = 0.5f;
	private float timeStamp;
	public float lastX = 1f;
	public float lastY = 0f;

    public GameObject bulletPrefab;
	public GameObject player;
    public Animator animator;

    public float timeBtwAttack;
    public float startTimeBtwAttack;
    private bool healing = false;

    private enum State 
    {
        Normal,
        Busy,
        Dead,
    }

    void Awake() 
    {
        playerHandler = this;
    }

    private void Setup(HealthSystem healthSystem, Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc) 
    {
        this.healthSystem = healthSystem;
        this.getClosestEnemyHandlerFunc = getClosestEnemyHandlerFunc;

        healthSystem.OnDead += HealthSystem_OnDead;

        attackPoint.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) 
    {
        state = State.Dead;
        gameObject.SetActive(false);
        if (OnDead != null) OnDead(this, EventArgs.Empty);
    }

    private void Update() 
    {
        switch (state) 
        {
        case State.Normal:
            HandleMovement();
            HandleAttack();
            HandleShooting();
            HandleHealing();
            HandleAoe();
            break;
        case State.Busy:
            HandleAttack();
            break;
        case State.Dead:
            GameHandler.Restart();
            break;
        }
    }

    private void HandleMovement() 
    {
        float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
        float xScale = transform.localScale.x;

        //Change x direction
        if (x == -1 && xScale != -1) {
            transform.localScale -= new Vector3(2, 0);

        } else if (x == 1 && xScale != 1) {
            transform.localScale += new Vector3(2, 0);
        }

        //stop colliding with enemies
        if (dashing == true) {
            this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 0.35f);
            gameObject.layer = 10;
        } else {
            this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 1f);
            gameObject.layer = 1;
        }

		if (dashing)
		{
			StartCoroutine(Dash());
			
		} else if(leaping)
		{
			StartCoroutine(Leap());

		} else
		{
			movement = new Vector2(x, y).normalized;
			rigidbody2D.velocity = movement * speed;

            if(movement == Vector2.zero) 
            {
                animator.SetInteger("Direction", 0);
                animator.SetBool("IsWalking", false);
            } 

			if(Input.GetButtonDown("Dash") && timeStamp <= Time.time)
			{
				dashing = true;
				Debug.Log("dashing");
			}

			if(Input.GetButtonDown("Leap") && timeStamp <= Time.time)
			{
				leaping = true;
				Debug.Log("leaping");
			}

			if (movement != new Vector2(lastX, lastY) && movement != Vector2.zero)
			{
				attackPoint.position = transform.position + (Vector3)(movement);
			}	
			
		}

		if(movement != Vector2.zero)
		{
            animator.SetBool("IsWalking", true);
		    lastX = x;
	    	lastY = y;
            if ((lastX == 1 || lastX == -1) && lastY == 0)
            {
                animator.SetInteger("Direction", 3);
            }
            if ((lastX != 1) && lastY == -1)
            {
                animator.SetInteger("Direction", 5);
            }       
        }
	}

	IEnumerator Dash()
	{   
		if(movement == Vector2.zero)
		{
			movement = new Vector2(lastX, lastY).normalized;
		}

		timeStamp = Time.time + recoveryTime;
		rigidbody2D.velocity = movement * dashSpeed;
		yield return new WaitForSeconds(dashTime);
		dashing = false;
	}

	IEnumerator Leap()
	{	
		if(movement == Vector2.zero)
		{
			movement = new Vector2(lastX, lastY).normalized;
		}

		timeStamp = Time.time + recoveryTime;
		rigidbody2D.velocity = (movement * -1) * dashSpeed * 1.5f;
		yield return new WaitForSeconds(dashTime);
		leaping = false;
	}
    
    private void HandleShooting() 
    {
        if (Input.GetButtonDown("Fire1")) 
        {
		    Instantiate(GameAssets.i.pfFireBall, player.transform.position, attackPoint.rotation);
	    }
    }

    private void HandleAttack() 
    {
        if (Input.GetButtonDown("Fire2") && timeBtwAttack <= 0) 
        {
            animator.SetBool("IsAttacking", true);

            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.tag == "EnemyRanged") 
                {
                   EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                   enemy.GetHealthSystem().Damage(20);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) 
                    {
                        enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    }        

                } else if (enemiesToDamage[i].gameObject.tag == "Enemy")
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(20);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) 
                    {
                        enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    } 
                }
            }

            timeBtwAttack = startTimeBtwAttack;

        } else
        {    
        timeBtwAttack -= Time.deltaTime;

        if (timeBtwAttack > 0) 
        {
        animator.SetBool("IsAttacking", false);
        }

        }
    } 

    private void HandleAoe() 
    {
         if (Input.GetKeyDown(KeyCode.M) && timeBtwAttack <= 0) 
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange * 3, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.tag == "EnemyRanged") 
                {
                   EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                   enemy.GetHealthSystem().Damage(50);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) 
                    {
                        enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    }        

                } else if (enemiesToDamage[i].gameObject.tag == "Enemy")
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(50);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) 
                    {
                        enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    } 
                }
            }

            timeBtwAttack = startTimeBtwAttack;

        } else
        {    
        timeBtwAttack -= Time.deltaTime;

        if (timeBtwAttack > 0) 
        {
        }

        }
    }

    private void HandleHealing()
    {
        if (Input.GetKeyDown(KeyCode.K) && !healing) {
            StartCoroutine(Healing());
        }
    }
    IEnumerator Healing()
	{	
        healing = true;
        healthSystem.Heal(25);
		yield return new WaitForSeconds(3);
        healing = false;
	}
    private void SetStateBusy() 
    {
        state = State.Busy;
    }

    private void SetStateNormal() 
    {
        state = State.Normal;
    }

    public bool IsDead() 
    {
        return state == State.Dead;
    }

    public Vector3 GetPosition() 
    {
        return transform.position;
    }

    public HealthSystem GetHealthSystem() 
    {
        return healthSystem;
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}
