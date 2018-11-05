﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler CreatePlayer(Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc) 
    {
        Transform playerTransform = Instantiate(GameAssets.i.pfPlayerTransform, new Vector3(0, 0), Quaternion.identity);
        
        HealthSystem healthSystem = new HealthSystem(150);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, new Vector3(0, 1.5f), Quaternion.identity, playerTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        PlayerHandler playerHandler = playerTransform.GetComponent<PlayerHandler>();
        playerHandler.Setup(healthSystem, getClosestEnemyHandlerFunc);

        return playerHandler;
    }

    #region 

    public static PlayerHandler playerHandler;

	public event EventHandler OnDead;
    public Transform attackPoint;

    public Rigidbody2D rb2d;
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

	public GameObject player;
    public Animator animator;

    public float timeBtwAttack;
    public float startTimeBtwAttack;
    private bool healing = false;
    private bool aoe = false;
    private bool isShooting = false;
    Image potion;
    Color potionColor;

    Image aoeFire;

    Image bullet;

    // public Text goldText;

    //upgradable values
    float healingCd = 3;
    float aoeCd = 7;
    float shootingCd = 2;

    //run values
    public int gold;
    Text goldText;
    GameObject goldTextObject;

    private int xp;

    Text xpText;
    GameObject xpTextObject;
    // GameControl gameControl;

    GameObject pc;

    
    #endregion

    private enum State 
    {
        Normal,
        Busy,
        Dead,
    }

    void Start()
    {
        goldTextObject =  GameObject.Find("CurrentGold");
        goldText = goldTextObject.GetComponent<Text>();

        xpTextObject =  GameObject.Find("CurrentXp");
        xpText = xpTextObject.GetComponent<Text>();

        // pc = GameObject.Find("PersistenceControl");
        // gameControl = pc.GetComponent<GameControl>();
        GameControl.control.Load();

        gold = GameControl.control.gold;
        xp = GameControl.control.xp;
        
        goldText.text = GameControl.control.gold.ToString(); 
        xpText.text = GameControl.control.xp.ToString();
    }

    void Awake() 
    {
        playerHandler = this;
        potion = GameAssets.i.potion;
        potionColor = potion.GetComponent<Image>().color;
        aoeFire = GameAssets.i.aoeFire;

        bullet = GameAssets.i.bullet;
        
        gold = 0;
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

        if (GetHealthSystem().GetHealthPercent() <= 0) IsDead();
    }

    private void HandleMovement() 
    {
        float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
        float xScale = transform.localScale.x;

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
			rb2d.velocity = movement * speed;

            if(movement == Vector2.zero) animator.SetBool("isWalking", false);

			if(Input.GetButtonDown("Dash") && timeStamp <= Time.time) dashing = true;
			if(Input.GetButtonDown("Leap") && timeStamp <= Time.time) leaping = true;

			if (movement != new Vector2(lastX, lastY) && movement != Vector2.zero)attackPoint.position = transform.position + (Vector3)(movement);	
			
		}

		if(movement != Vector2.zero)
		{
            animator.SetBool("isWalking", true);
		    lastX = x;
	    	lastY = y;

            //right
            if (lastX == 1 && lastY == 0) animator.SetInteger("Direction", 3);
            //left
            if (lastX == -1 && lastY == 0) animator.SetInteger("Direction", 7);
            //down
            if ((lastX != 1) && lastY == -1) animator.SetInteger("Direction", 5);  
        }
	}

	IEnumerator Dash()
	{   
		if(movement == Vector2.zero) movement = new Vector2(lastX, lastY).normalized;

		timeStamp = Time.time + recoveryTime;
		rb2d.velocity = movement * dashSpeed;
		yield return new WaitForSeconds(dashTime);
		dashing = false;
	}

	IEnumerator Leap()
	{	
		if(movement == Vector2.zero) movement = new Vector2(lastX, lastY).normalized;

		timeStamp = Time.time + recoveryTime;
		rb2d.velocity = (movement * -1) * dashSpeed * 1.5f;
		yield return new WaitForSeconds(dashTime);
		leaping = false;
	}
    
    private void HandleShooting() 
    {
        if ((Input.GetButtonDown("Fire") || Input.GetAxisRaw("FireController") == 1) && !isShooting) StartCoroutine(Shooting());
    }

    IEnumerator Shooting()
    {
        StartCoroutine(FadeToS(0f, shootingCd, bullet.GetComponent<Image>().color));
        Instantiate(GameAssets.i.pfFireBall, player.transform.position, attackPoint.rotation);
        isShooting = true;
        yield return new WaitForSeconds(shootingCd);
        isShooting = false;
    }

    IEnumerator FadeToS(float aValue, float aTime, Color cooldownColor)
    {
        StartCoroutine(BulletCdColor());
        float alpha = cooldownColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(aValue, alpha,t));
            bullet.GetComponent<Image>().color = newColor;
            yield return null;
        }
    }
    IEnumerator BulletCdColor()
    {
        yield return new WaitForSeconds(shootingCd);
        bullet.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    private void HandleAttack() 
    {
        if (Input.GetButtonDown("Basic") && timeBtwAttack <= 0) 
        {
            animator.SetTrigger("Attack");

            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                   EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                   enemy.GetHealthSystem().Damage(20);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    // if (enemy.GetHealthSystem().GetHealthPercent() <= 0) GetGold(5);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(20);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                }
            }

            timeBtwAttack = startTimeBtwAttack;

        } else
        {    
        timeBtwAttack -= Time.deltaTime;

        }
    } 

    private void HandleAoe() 
    {
         if (Input.GetButtonDown("AoE") && aoe == false) StartCoroutine(AoE());     
    }

    IEnumerator AoE() 
    {
        aoe = true;
        Instantiate(GameAssets.i.pfCircle, transform.position, Quaternion.identity);
        StartCoroutine(FadeToF(aoeCd, aoeFire.GetComponent<Image>().color));

        // cooldownTimerScript.timeLeft = aoeCd;
            // cooldownTimer.SetActive(true);

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange * 4, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                    EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                    enemy.GetHealthSystem().Damage(50);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(50);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        yield return new WaitForSeconds(aoeCd);
        aoe = false;
    }
    IEnumerator FadeToF(float aTime, Color cooldownColor)
    {
        StartCoroutine(AoeCdColor());
        StartCoroutine(DeletePf());
        float alpha = cooldownColor.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(0f, alpha,t));
            aoeFire.GetComponent<Image>().color = newColor;
            yield return null;
        }
    }

    IEnumerator AoeCdColor()
    {
        yield return new WaitForSeconds(aoeCd);
        aoeFire.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    IEnumerator DeletePf() 
    {
        GameObject toDelete = GameObject.FindWithTag("Circle");
        yield return new WaitForSeconds(1);
        Destroy(toDelete);       
    }
    private void HandleHealing()
    {
        if (Input.GetButtonDown("Heal") && !healing) StartCoroutine(Healing());
    }
    IEnumerator Healing()
	{	
        healing = true;
        healthSystem.Heal(25);
        StartCoroutine(FadeTo(0f, (float) healingCd, potionColor));
		yield return new WaitForSeconds(healingCd);
        healing = false;
	}

    IEnumerator FadeTo(float aValue, float aTime, Color cooldownColor)
    {
        StartCoroutine(HealCdColor());
        float alpha = cooldownColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(aValue, alpha,t));
            potion.GetComponent<Image>().color = newColor;
            yield return null;
        }
    }

    IEnumerator HealCdColor()
    {
        yield return new WaitForSeconds(healingCd);
        potion.GetComponent<Image>().color = new Color(1, 1, 1, 1);

    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
	{
		if(hitInfo.gameObject.CompareTag("Bomb")) 
		{
			this.GetHealthSystem().Damage(50);
			this.rb2d.AddForce(700 * -movement * speed); //wth
            // this.rb2d.AddExplosionForce(10, hitInfo.transform.position, 5, 3);
			Destroy(hitInfo.gameObject);
		}
	}

    public void GetRewards(int newGold, int newXp)
    {
        gold += newGold;
        xp += newXp;
        goldText.text = gold.ToString();
        xpText.text = xp.ToString();
    }

    public void SaveRewards()
    {
        GameControl.control.xp = xp;
        GameControl.control.gold = gold;
        GameControl.control.Save();
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
        Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, attackRange * 4);
	}
}