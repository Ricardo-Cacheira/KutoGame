 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler CreatePlayer() 
    {
        Transform playerTransform = Instantiate(GameAssets.i.pfPlayerTransform, new Vector3(0, 0), Quaternion.identity);
        
        HealthSystem healthSystem = new HealthSystem(150 + (GameControl.control.lvl * 2));
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
    public Transform stunPoint;

    public Rigidbody2D rb2d;
	public float attackRange;
    public LayerMask whatIsEnemies;

    private HealthSystem healthSystem;
    private ExperienceSystem experienceSystem;
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
    private bool hasFoundIntel = false;

    Image potion;
    Image aoeFire;
    Image bullet;
    Image dash;

    //upgradable values
    int basicAtkDmg = 20;
    int healingAmount = 30;
    float healingCd = 5;
    int aoeDmg = 36;
    float aoeCd = 7.5f;
    public int shootingDmg = 30;
    float shootingCd = 2;
    float stunCd = 6;

    int bombDmg = 30;

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

    //Audio

    public AudioManager audioManager;
    Animator animatorStun;
    private bool moving;
    Camera cam;
    AnimatorClipInfo[] m_CurrentClipInfo;

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
        
        // basicAtkDmg += GameControl.control.lvl;
        // aoeDmg += GameControl.control.lvl;
        // shootingDmg += GameControl.control.lvl;
        // healingAmount += GameControl.control.lvl;
        float tmpLvl = GameControl.control.lvl;
        double p = Mathf.Pow(tmpLvl, 0.5f);
        Debug.Log(p);
        basicAtkDmg += (int) p * 3;
        aoeDmg += (int) p;
        shootingDmg += (int) p;
        healingAmount += (int) p;

        stunPoint.GetComponent<StunCollider>().enabled = false;
        stunPoint.GetComponent<BoxCollider2D>().enabled = false;

        moving = true;

        cam = FindObjectOfType<Camera>();

        if (FindObjectOfType<Joystick>() != null) joystick = GameObject.Find("WalkingJoystick").GetComponent<Joystick>();
    }

    private void Setup(HealthSystem healthSystem, ExperienceSystem experienceSystem) 
    {
        this.healthSystem = healthSystem;
        this.experienceSystem = experienceSystem;

        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnHealthDecrease += HealthSystem_OnHealthDecrease;

        SetupSkills();
        for (int i = 0; i < GameControl.control.equippedItems.Count; i++)
        {
            itemSkills[i] = GameControl.control.equippedItems[i].skillID;
        }
        SetupSkillIcons();

        audioManager = FindObjectOfType<AudioManager>();

        Scene currScene = SceneManager.GetActiveScene();
        if (currScene.name == "Prototype") audioManager.Play("Jungle", true);
        if (currScene.name == "BeachScene") audioManager.Play("Beach", true);   
    }

    private void HealthSystem_OnHealthDecrease(object sender, EventArgs e)
    {
        audioManager.Play("Hurt");
    }
    private void HealthSystem_OnDead(object sender, EventArgs e) 
    {
        state = State.Dead;
        gameObject.SetActive(false);
        if (OnDead != null) OnDead(this, EventArgs.Empty);
    }

    private void SetupSkillIcons()
    {
        potion.enabled = false;
        bullet.enabled = false;
        aoeFire.enabled = false;
        for (int i = 0; i < GameControl.control.cooldowns.Length; i++)
        {
            switch(GameControl.control.cooldowns[i])
            {
                case 0:
                    break;
                case 1:
                    potion.enabled = true;
                    potion.rectTransform.anchoredPosition = new Vector3(-225 + (100*i),21.5f,0);
                    break;
                case 2:
                    aoeFire.enabled = true;
                    aoeFire.rectTransform.anchoredPosition = new Vector3(-225 + (100*i),21.5f,0);
                    break;
                case 3:
                    bullet.enabled = true;
                    bullet.rectTransform.anchoredPosition = new Vector3(-225 + (100*i),21.5f,0);
                    break;
                case 4:
                    //stun
                    break;
                default: break;
            }

        }
    }

    private void SetupSkills()
    {
        skills.Add(0, None);
        skills.Add(1, HandleHealing);
        skills.Add(2, HandleAoe);
        skills.Add(3, HandleShooting);
        skills.Add(4, Stun);
    }

    void None()
    {return;}


    void Update() 
    {
        if (Input.GetMouseButton(0))
        {
        var mousePos = Input.mousePosition;
        mousePos.z = 100; // select distance = 10 units from the camera
        CreateText(Color.green, mousePos, new Vector2(0, 5f), "hello");
        }

        if (Input.GetMouseButton(1))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 100; // select distance = 10 units from the camera
            CreateText(Color.yellow, mousePos, new Vector2(0, 5f), "BITCONNECT");
        }

        //TESTING PURPOSES//
		if (Input.GetKeyDown("p")) basicAtkDmg = 1000;
        if (Input.GetKeyDown("n")) GetRewards(0, 100);
        //---------------//
        
        switch (state) 
        {
        case State.Normal:
            HandleMovement();
            HandleAttack();
            Stun();
            
            if((Input.GetButtonDown("Fire") || Input.GetAxisRaw("FireController") == 1 || phoneShooting))
                {skills[GameControl.control.cooldowns[0]](); phoneShooting = false;}
            else
                phoneShooting = false;

            if(Input.GetButtonDown("AoE") || phoneAoe)
                {skills[GameControl.control.cooldowns[1]](); phoneAoe=false;}
            else 
                phoneAoe = false;
                
            if (Input.GetButtonDown("Heal") || phoneHeal)
                {skills[GameControl.control.cooldowns[2]](); phoneHeal=false;}
            else
                phoneHeal = false;
            break;
        case State.Busy:
            HandleAttack();
            break;
        case State.Dead:
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

		if (dashing)
		{
			StartCoroutine(Dash());
            movement = new Vector2(x, y).normalized;

		} else if(moving)
		{
			movement = new Vector2(x, y).normalized;
			rb2d.velocity = movement * speed;

			if((Input.GetButtonDown("Dash") || phoneDash) && timeStamp <= Time.time) {
                dashing = true;
                audioManager.Play("Dash");

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
            animator.SetFloat("WalkingH", x);
            animator.SetFloat("WalkingV", y);
		    lastX = x;
	    	lastY = y;  
        }  else {
            animator.SetBool("isWalking", false);
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
    
    private void HandleShooting() 
    {
        if (!isShooting)
            StartCoroutine(Shooting());   
    }

    IEnumerator Shooting()
    {
        audioManager.Play("Bullet");
        // phoneShooting = false;
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
            StartCoroutine(TmpBusy(0.1f));
            animator.SetTrigger("Attack");
            phoneAttack = false;
            
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemies);      
            if (enemiesToDamage.Length > 0) 
            {
        
                audioManager.Play("SlashSuccess");
                
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                    {
                        EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                        enemy.GetHealthSystem().Damage(basicAtkDmg); 
                        enemy.KnockBack(200000);
                        
                        CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + basicAtkDmg);

                        if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                    } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                    {
                        EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                        enemy.GetHealthSystem().Damage(basicAtkDmg);
                        enemy.KnockBack(200000);

                        CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + basicAtkDmg);

                        if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                    } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                    {
                        EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                        enemy.GetHealthSystem().Damage(basicAtkDmg);
                        enemy.KnockBack(200000);
                        
                        CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + basicAtkDmg);

                        if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    } else if (enemiesToDamage[i].gameObject.CompareTag("Boss"))
                    {
                        BossHandler boss = enemiesToDamage[i].GetComponent<BossHandler>();  
                        boss.GetHealthSystem().Damage(basicAtkDmg);
                        
                        CreateText(Color.grey, new Vector3(boss.transform.position.x, boss.transform.position.y + 1), new Vector2(0, 5), "-" + basicAtkDmg);

                        if (boss.GetHealthSystem().GetHealthPercent() < 0.1) boss.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                    }
                } 
            } else 
            {
                audioManager.Play("SlashFail");

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
        if (aoe == false) 
            StartCoroutine(AoE());
        
    }

    IEnumerator AoE() 
    {
        // phoneAoe = false;
        aoe = true;
        audioManager.Play("Explosion");
        gameObject.GetComponent<ParticleSystem>().Play();

        // Transform tempAoe = Instantiate(GameAssets.i.pfCircle, transform.position, Quaternion.identity);

        StartCoroutine(FadeToF(aoeCd, aoeFire.GetComponent<Image>().color));

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange * 4, whatIsEnemies);      

            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].gameObject.CompareTag("EnemyRanged")) 
                {
                    EnemyRangedHandler enemy = enemiesToDamage[i].GetComponent<EnemyRangedHandler>();
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

                } else if (enemiesToDamage[i].gameObject.CompareTag("Enemy"))
                {
                    EnemyHandler enemy = enemiesToDamage[i].GetComponent<EnemyHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);

                    CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("EnemySlower"))
                {
                    EnemySlowerHandler enemy = enemiesToDamage[i].GetComponent<EnemySlowerHandler>();  
                    enemy.GetHealthSystem().Damage(aoeDmg);
                    enemy.KnockBack(1000000);
                    
                    CreateText(Color.grey, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1), new Vector2(0, 5), "-" + aoeDmg);
                    if (enemy.GetHealthSystem().GetHealthPercent() < 0.25) enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                } else if (enemiesToDamage[i].gameObject.CompareTag("Boss"))
                {
                    BossHandler boss = enemiesToDamage[i].GetComponent<BossHandler>();  
                    boss.GetHealthSystem().Damage(aoeDmg);
                    
                    CreateText(Color.grey, new Vector3(boss.transform.position.x, boss.transform.position.y + 1), new Vector2(0, 5), "-" + aoeDmg);
                    if (boss.GetHealthSystem().GetHealthPercent() < 0.25) boss.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        // Destroy(tempAoe.gameObject, 0.3f);
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

    private void Stun()
    {
        if(Input.GetKeyDown("m") && stunCd <= 0)
        {
            stunPoint.GetComponent<BoxCollider2D>().enabled = true;
            stunPoint.GetComponent<Animator>().SetTrigger("Stun");
            stunCd = 5;
            stunPoint.position = transform.position + (new Vector3(lastX, lastY,0)) * 3.5f;
            float rot_z = Mathf.Atan2(lastY, lastX) * Mathf.Rad2Deg;
            stunPoint.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

            StartCoroutine(TmpBusy(0.3f));
            StartCoroutine(FadeCrack());

        } else
            stunCd -= Time.deltaTime;
    }

    private IEnumerator TmpBusy(float time)
    {
        rb2d.velocity = Vector3.zero;
        moving = false;
        yield return new WaitForSeconds(time);
        speed = 6;
        moving = true;
    }

    private IEnumerator FadeCrack()
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            Color newColor = new Color(.35f, .35f, .35f, Mathf.Lerp(2.5f, 0, t));
            stunPoint.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        stunPoint.GetComponent<BoxCollider2D>().enabled = false;

    }

    private void HandleHealing()
    {
        if (!healing) 
            StartCoroutine(Healing());
    }

    IEnumerator Healing()
	{
        audioManager.Play("Potion");
        // phoneHeal = false;
        healing = true;
        healthSystem.Heal(healingAmount);
        CreateText(Color.green, new Vector3(transform.position.x, transform.position.y + 1), new Vector2(0, 5f),"+" + healingAmount);
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
			Explosion(hitInfo, bombDmg + (GameControl.control.lvl * 2));
            CreateText(Color.grey, new Vector3(transform.position.x, transform.position.y + 1), new Vector2(0, 5f), "-" + bombDmg);	
		}
        
		if (hitInfo.gameObject.CompareTag("Circle"))
		{
            Explosion(hitInfo, BossHandler.dmgAoe);
            CreateText(Color.grey, new Vector3(transform.position.x, transform.position.y + 1), new Vector2(0, 5f), "-" + BossHandler.dmgAoe);
        }
	}

    private void Explosion(Collider2D col, int dmg)
    {
        this.GetHealthSystem().Damage(dmg);
		this.rb2d.AddForce(900 * -movement * speed);
        col.GetComponent<ParticleSystem>().Play();
        col.GetComponent<SpriteRenderer>().enabled = false;
        col.GetComponent<CircleCollider2D>().enabled = false;
        Destroy(col.gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Intel") && GameHandler.noEnemies && !hasFoundIntel)
        {
            var reward = GameObject.FindWithTag("reward");
            Sprite item = GameControl.control.DropItem().icon;
            reward.GetComponent<SpriteRenderer>().sprite = item;
            GetRewards(1000, 0);
            SaveRewards();
            StartCoroutine(GameHandler.WinMessage());
            hasFoundIntel = true;
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
        canvasObj = GameObject.FindGameObjectWithTag("CanvasMobile");
        tempTextBox = Instantiate(GameAssets.i.combatText, pos, transform.rotation);
        tempTextBox.transform.SetParent(canvasObj.transform, true);
        tempTextBox.transform.position = RectTransformUtility.WorldToScreenPoint(cam, pos);
        // tempTextBox.transform.position = cam.ScreenToWorldPoint(pos);
        tempTextBox.GetComponent<Text>().color = color;
        tempTextBox.GetComponent<CombatText>().Initialize(2, displayDmg, dir * 12);
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
