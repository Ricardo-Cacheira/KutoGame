 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler CreatePlayer() 
    {
        Transform playerTransform = Instantiate(GameAssets.i.pfPlayerTransform, new Vector3(0, 0), Quaternion.identity);
        
        HealthSystem healthSystem = new HealthSystem(200);
        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, new Vector3(0, 1.5f), Quaternion.identity, playerTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        ExperienceSystem experienceSystem = new ExperienceSystem(GameControl.control.tempXp);
        Transform experienceBarObj = Instantiate(GameAssets.i.pfXpBar, new Vector3(0, 0), Quaternion.identity);
        ExperienceBar experienceBar = experienceBarObj.GetComponent<ExperienceBar>();
        experienceBarObj.SetParent(GameObject.Find("XpBarHolder").GetComponent<Transform>(), false);
        experienceBar.Setup(experienceSystem);  

        PlayerHandler playerHandler = playerTransform.GetComponent<PlayerHandler>();
        playerHandler.Setup(healthSystem, experienceSystem);

        return playerHandler;
    }

    #region VARIABLES
    
    [SerializeField] int[] itemSkills = new int[4];
    private Dictionary<int, Action> skills = new Dictionary<int, Action>();

    public static PlayerHandler playerHandler;
    private Joystick joystick;

	public event EventHandler OnDead;
    public Transform attackPoint;

    bool dropped;

    public Rigidbody2D rb2d;
	public float attackRange;
	public int damage;
    public LayerMask whatIsEnemies;

    private HealthSystem healthSystem;
    private ExperienceSystem experienceSystem;
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
    Image dash;

    //upgradable values
    int basicAtkDmg = 20;

    int healingAmount = 45;
    float healingCd = 3;

    int aoeDmg = 49;
    float aoeCd = 7;

    int shootingDmg = 40;
    float shootingCd = 2;

    private float randDir;

    //rewards values
    private int gold;
    Text goldText;
    GameObject goldTextObject;

    public int xp;

    Text xpText;
    GameObject xpTextObject;

    Text lvlText;
    GameObject lvlTextObject;

    Text xpPercentageText;
    GameObject xpPercentageTextObject;

    GameObject canvasObj;
    RectTransform tempTextBox;

    //phone considerations
    public static bool phoneAttack = false;
    public static bool phoneShooting, phoneAoe, phoneHeal, phoneDash;
    public Input input;

    #endregion

    private enum State 
    {
        Normal,
        Busy,
        Dead,
    }

    void Awake() 
    {
        playerHandler = this;
        
        aoeFire = GameAssets.i.aoeFire;
        bullet = GameAssets.i.bullet;
        potion = GameAssets.i.potion;
        dash = GameAssets.i.dash;
    }

    void Start()
    {
        goldTextObject =  GameObject.Find("CurrentGold");
        goldText = goldTextObject.GetComponent<Text>();

        xpTextObject =  GameObject.Find("CurrentXp");
        xpText = xpTextObject.GetComponent<Text>();

        xpPercentageTextObject = GameObject.Find("XpBarHolder");
        xpPercentageText = xpPercentageTextObject.GetComponent<Text>();

        lvlTextObject = GameObject.Find("LvlText");
        lvlText = lvlTextObject.GetComponent<Text>();
        lvlText.text = GameControl.control.lvl.ToString();

        xp = GameControl.control.xp;

        gold = GameControl.control.gold;
        goldText.text = GameControl.control.gold.ToString(); 

        xpText.text = GameControl.control.xp.ToString();

        float tempPercentage = experienceSystem.GetXpPercent() * 100;
        xpPercentageText.text = Math.Round((tempPercentage), 1)  + "%";

        canvasObj = GameObject.FindGameObjectWithTag("Canvas");

        basicAtkDmg += GameControl.control.lvl * 2;
        aoeDmg += GameControl.control.lvl * 2;
        shootingDmg += GameControl.control.lvl * 2;

        joystick = GameObject.Find("WalkingJoystick").GetComponent<Joystick>();
        
    }

    private void Setup(HealthSystem healthSystem, ExperienceSystem experienceSystem) 
    {
        this.healthSystem = healthSystem;
        this.experienceSystem = experienceSystem;

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

    void Update() 
    {
        switch (state) 
        {
        case State.Normal:
            HandleMovement();
            HandleAttack();
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

        if (joystick != null && joystick.Direction != Vector2.zero) {
            x = joystick.Horizontal;
            y = joystick.Vertical;
        }

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

			if((Input.GetButtonDown("Dash") || phoneDash) && timeStamp <= Time.time) {
                dashing = true;
                StartCoroutine(FadeToD(2, dash.GetComponent<Image>().color));
            }
            else
                phoneDash = false;

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
        phoneDash = false;
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

    IEnumerator FadeToD(float aTime, Color cooldownColor)
    {
        StartCoroutine(DashCdColor());
        float alpha = cooldownColor.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(0f, alpha,t));
            dash.GetComponent<Image>().color = newColor;
            yield return null;
        }
    }

    IEnumerator DashCdColor()
    {
        yield return new WaitForSeconds(2);
        dash.GetComponent<Image>().color = new Color(1, 1, 1, 1);
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
        if ((Input.GetButtonDown("Fire") || Input.GetAxisRaw("FireController") == 1 || phoneShooting) && !isShooting)
            StartCoroutine(Shooting());
        else 
            phoneShooting = false;
            
    }

    IEnumerator Shooting()
    {
        phoneShooting = false;
        StartCoroutine(FadeToS(0f, shootingCd, bullet.GetComponent<Image>().color));
        Instantiate(GameAssets.i.pfFireBall, player.transform.position, Quaternion.identity);
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
        if ((Input.GetButtonDown("Basic") || phoneAttack) && timeBtwAttack <= 0) 
        {   
            animator.SetTrigger("Attack");
            phoneAttack = false;
            
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                randDir = UnityEngine.Random.Range(1.5f, 4.5f);
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                    EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                    enemy.GetHealthSystem().Damage(basicAtkDmg); 
                    enemy.KnockBack(200000);
                    
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(randDir, randDir), "-" + basicAtkDmg);

                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(basicAtkDmg);
                    enemy.KnockBack(200000);

                    CreateText(Color.green, playerHandler.transform.position, new Vector2(randDir, randDir), "-" + basicAtkDmg);

                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                {
                    EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                    enemy.GetHealthSystem().Damage(basicAtkDmg);
                    enemy.KnockBack(200000);
                    
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(randDir, randDir), "-" + basicAtkDmg);

                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("Boss"))
                {
                    BossHandler boss = enemiesToDamage[i].GetComponent<BossHandler>();  
                    boss.GetHealthSystem().Damage(basicAtkDmg);
                    
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(randDir, randDir), "-" + basicAtkDmg);

                    if (boss.GetHealthSystem().GetHealthPercent() < 0.1) boss.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } 
            }
                timeBtwAttack = startTimeBtwAttack;

        } else
        { 
        phoneAttack = false;
        timeBtwAttack -= Time.deltaTime;

        }
    } 

    private void HandleAoe() 
    {
        if ((Input.GetButtonDown("AoE") || phoneAoe) && aoe == false) 
            StartCoroutine(AoE());     
        else 
            phoneAoe = false;
    }

    IEnumerator AoE() 
    {
        phoneAoe = false;
        aoe = true;
        Transform tempAoe = Instantiate(GameAssets.i.pfCircle, transform.position, Quaternion.identity);
        StartCoroutine(FadeToF(aoeCd, aoeFire.GetComponent<Image>().color));

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange * 4, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                    EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    randDir = UnityEngine.Random.Range(1.5f, 4.5f);
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, randDir), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    randDir = UnityEngine.Random.Range(1.5f, 4.5f);
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, randDir), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                {
                    EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);
                    
                    randDir = UnityEngine.Random.Range(1.5f, 4.5f);
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, randDir), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("Boss"))
                {
                    BossHandler boss = enemiesToDamage[i].GetComponent<BossHandler>();  
                    boss.GetHealthSystem().Damage(aoeDmg);
                    
                    randDir = UnityEngine.Random.Range(1.5f, 4.5f);
                    CreateText(Color.green, playerHandler.transform.position, new Vector2(1, randDir), "-" + aoeDmg);
                    if (boss.GetHealthSystem().GetHealthPercent() < 0.25) boss.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        Destroy(tempAoe.gameObject, 0.3f);
        yield return new WaitForSeconds(aoeCd);
        aoe = false;

    }
    IEnumerator FadeToF(float aTime, Color cooldownColor)
    {
        StartCoroutine(AoeCdColor());
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

    private void HandleHealing()
    {
        if ((Input.GetButtonDown("Heal") || phoneHeal) && !healing) 
            StartCoroutine(Healing());
        else
            phoneHeal = false;
    }


    IEnumerator Healing()
	{	
        phoneHeal = false;
        healing = true;
        healthSystem.Heal(healingAmount);
        randDir = UnityEngine.Random.Range(1.5f, 4.5f);
        CreateText(Color.green, playerHandler.transform.position, new Vector2(1f, randDir),"+" + healingAmount);
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
            hitInfo.GetComponent<CircleCollider2D>().enabled = false;

            CreateText(Color.red, transform.position, new Vector2(-1, 4.5f), "-" + 50);

            if (this.GetHealthSystem().GetHealthPercent() <= 0) {
                GameHandler.Restart();
            }

			Destroy(hitInfo.gameObject, 1f);
		}

        
		if (hitInfo.gameObject.CompareTag("Circle"))
		{
			this.GetHealthSystem().Damage(BossHandler.dmgAoe);;
			this.rb2d.AddForce(900 * -movement * speed); //wth
            hitInfo.GetComponent<ParticleSystem>().Play();

            hitInfo.GetComponent<SpriteRenderer>().enabled = false;
            hitInfo.GetComponent<CircleCollider2D>().enabled = false;

            CreateText(Color.red, transform.position, new Vector2(-1, 3.5f), "-" + BossHandler.dmgAoe);

            if (this.GetHealthSystem().GetHealthPercent() <= 0) {
                GameHandler.Restart();
            }
            
			Destroy(hitInfo.gameObject, 1f);
		}
	}

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Intel") && GameHandler.noEnemies && !dropped)
        {
            var reward = GameObject.FindWithTag("reward");
            Sprite item = GameControl.control.DropItem().icon;
            reward.GetComponent<SpriteRenderer>().sprite = item;
            GetRewards(1000, 0);
            SaveRewards();
            StartCoroutine(GameHandler.WinMessage());
            dropped = true;
        }
    }

    public void GetRewards(int newGold, int newXp)
    {
        gold += newGold;
        goldText.text = gold.ToString();
        
        experienceSystem.WinXp(newXp);
        float tempPercentage = (experienceSystem.GetXpPercent() * 100.0f);

            
        if (GameControl.control.isXpMax == false) {
            xp += newXp;
            xpText.text = GameControl.control.xp.ToString(); 
            xpPercentageText.text = Math.Round((tempPercentage), 1)  + "%";
        } else 
        {
            xpText.text = GameControl.control.xp.ToString();
            xpPercentageText.text = Math.Round((tempPercentage), 1) + "%";

        }
        
    }

    public void SaveRewards()
    {
        GameControl.control.xp = xp;
        GameControl.control.gold = gold;
        GameControl.control.Save();
    }

    public void CreateText(Color color, Vector3 pos, Vector2 dir, String displayDmg)
    {
        canvasObj = GameObject.FindGameObjectWithTag("Canvas");
        tempTextBox = Instantiate(GameAssets.i.combatText, new Vector3(pos.x, pos.y + 30, 0), transform.rotation);
        tempTextBox.transform.SetParent(canvasObj.transform, false);
        tempTextBox.GetComponent<Text>().color = color;
        tempTextBox.GetComponent<CombatText>().Initialize(2, displayDmg, dir);
    }

    public void KnockBack(float force, Vector3 pos)
    {   
        Vector2 knock = (this.transform.position - pos).normalized;
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

    public ExperienceSystem GetExperienceSystem()
    {
        return experienceSystem;
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, attackRange * 4);
	}
}
