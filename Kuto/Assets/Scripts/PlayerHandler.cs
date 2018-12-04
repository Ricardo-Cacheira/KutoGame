using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler CreatePlayer(Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc) 
    {
        Transform playerTransform = Instantiate(GameAssets.i.pfPlayerTransform, new Vector3(0, 0), Quaternion.identity);
        
        HealthSystem healthSystem = new HealthSystem(200);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, new Vector3(0, 1.5f), Quaternion.identity, playerTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        PlayerHandler playerHandler = playerTransform.GetComponent<PlayerHandler>();
        playerHandler.Setup(healthSystem, getClosestEnemyHandlerFunc);

        return playerHandler;
    }

    #region VARIABLES
    
    [SerializeField] int[] itemSkills = new int[4];
    private Dictionary<int, Action> skills = new Dictionary<int, Action>();

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
	public float recoveryTime = 2f;
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

    //upgradable values
    int basicAtkDmg = 20;

    int healingAmount = 45;
    float healingCd = 3;

    int aoeDmg = 50;
    float aoeCd = 7;

    int shootingDmg = 40;
    float shootingCd = 2;

    //rewards values
    private int gold;
    Text goldText;
    GameObject goldTextObject;

    private int xp;

    Text xpText;
    GameObject xpTextObject;
    // GameControl gameControl;
    GameObject canvasObj;
    RectTransform tempTextBox;


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
        gold = GameControl.control.gold;
        xp = GameControl.control.xp;
        goldText.text = GameControl.control.gold.ToString(); 
        xpText.text = GameControl.control.xp.ToString();

        canvasObj = GameObject.FindGameObjectWithTag("Canvas");
    }

    void Awake() 
    {
        playerHandler = this;
        
        aoeFire = GameAssets.i.aoeFire;
        bullet = GameAssets.i.bullet;
        potion = GameAssets.i.potion;
    }

    private void Setup(HealthSystem healthSystem, Func<Vector3, EnemyHandler> getClosestEnemyHandlerFunc) 
    {
        this.healthSystem = healthSystem;
        this.getClosestEnemyHandlerFunc = getClosestEnemyHandlerFunc;

        healthSystem.OnDead += HealthSystem_OnDead;

        SetupSkills();
        for (int i = 0; i < GameControl.control.equippedItems.Count; i++)
        {
            itemSkills[i] = GameControl.control.equippedItems[i].skillID;
        }
        SetupSkillIcons();


        attackPoint.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
    }

    private void SetupSkillIcons()
    {
        if (Array.IndexOf(itemSkills, 1) > -1)
            potion.enabled = true;
        else
            potion.enabled = false;

        if (Array.IndexOf(itemSkills, 2) > -1)
            aoeFire.enabled = true;
        else
            aoeFire.enabled = false;
            
        if (Array.IndexOf(itemSkills, 3) > -1)
            bullet.enabled = true;
        else
            bullet.enabled = false;
        
    }

    private void SetupSkills()
    {
        skills.Add(0, None);
        skills.Add(1, HandleHealing);
        skills.Add(2, HandleAoe);
        skills.Add(3, HandleShooting);
    }

    void None()
    {return;}

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
            // HandleShooting();
            // HandleHealing();
            // HandleAoe();
            skills[itemSkills[0]]();
            skills[itemSkills[1]]();
            skills[itemSkills[2]]();
            skills[itemSkills[3]]();
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

        animator.SetFloat("WalkingH", x);
        animator.SetFloat("WalkingV", y);

        //stop colliding with enemies
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

			if (movement != new Vector2(lastX, lastY) && movement != Vector2.zero)
                attackPoint.position = transform.position + (Vector3)(movement);	
			
		}

		if(movement != Vector2.zero)
		{
            animator.SetBool("isWalking", true);
		    lastX = x;
	    	lastY = y;  
        }
	}


	IEnumerator Dash()
	{   
		if(movement == Vector2.zero) movement = new Vector2(lastX, lastY).normalized;
        animator.speed = 5;
		timeStamp = Time.time + recoveryTime;
		rb2d.velocity = movement * dashSpeed;
        Color tmp = this.GetComponent<SpriteRenderer>().color;
        tmp.a = .35f;
        this.GetComponent<SpriteRenderer>().color = tmp;
        gameObject.layer = 15;
		yield return new WaitForSeconds(dashTime);
        animator.speed = 1;
		dashing = false;
        tmp.a = 1f;
        this.GetComponent<SpriteRenderer>().color = tmp;
        gameObject.layer = 1;
	}

	IEnumerator Leap()
	{	
		if(movement == Vector2.zero) movement = new Vector2(lastX, lastY).normalized;
        animator.speed = 0;
		timeStamp = Time.time + recoveryTime;
		rb2d.velocity = (movement * -1) * dashSpeed * 1.5f;
        this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 0.35f);
        gameObject.layer = 15;
		yield return new WaitForSeconds(dashTime);
		animator.speed = 1;
        leaping = false;
        this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 1f);
        gameObject.layer = 1;
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
                    enemy.GetHealthSystem().Damage(basicAtkDmg); 
                    enemy.KnockBack(200000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + basicAtkDmg);

                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(basicAtkDmg);
                    enemy.KnockBack(200000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + basicAtkDmg);

                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                {
                    EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                    enemy.GetHealthSystem().Damage(basicAtkDmg);
                    enemy.KnockBack(200000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + basicAtkDmg);

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

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange * 4, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                    EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                {
                    EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, 2.5f), "-" + aoeDmg);
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
        healthSystem.Heal(healingAmount);
        CreateText(Color.green, playerHandler.transform.position, new Vector2(1f, 3f),"+" + healingAmount);
        StartCoroutine(FadeTo(healingCd, potion.GetComponent<Image>().color));
		yield return new WaitForSeconds(healingCd);
        healing = false;
	}

    IEnumerator FadeTo(float aTime, Color cooldownColor)
    {
        StartCoroutine(HealCdColor());
        float alpha = cooldownColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(0f, alpha,t));
            potion.GetComponent<Image>().color = newColor;
            yield return null;
        }
    }

    IEnumerator HealCdColor()
    {
        yield return new WaitForSeconds(healingCd);
        potion.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void SlowPlayer()
    {
        StartCoroutine(Slowed());
    }

    IEnumerator Slowed()
    {
        speed = 1.5f;
        this.GetComponent<SpriteRenderer>().color = new Color (0.3f, 1f, 1f, 1f);
        yield return new WaitForSeconds(2.5f);
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        speed = 6f;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
	{
		if(hitInfo.gameObject.CompareTag("Bomb")) 
		{
			this.GetHealthSystem().Damage(50);
			this.rb2d.AddForce(1000 * -movement * speed); //wth
            hitInfo.GetComponent<ParticleSystem>().Play();

            hitInfo.GetComponent<SpriteRenderer>().enabled = false;
            hitInfo.GetComponent<BoxCollider2D>().enabled = false;
            
			Destroy(hitInfo.gameObject, 1f);
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

    public void CreateText(Color color, Vector3 pos, Vector2 dir, String displayDmg)
    {
        canvasObj = GameObject.FindGameObjectWithTag("Canvas");;
        tempTextBox = Instantiate(GameAssets.i.combatText, new Vector3(pos.x, pos.y + 30, 0), transform.rotation);
        tempTextBox.transform.SetParent(canvasObj.transform, false);
        tempTextBox.GetComponent<Text>().color = color;
        tempTextBox.GetComponent<CombatText>().Initialize(2, displayDmg, dir);
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
