using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeMonkey.Utils;

public class BossHandler : MonoBehaviour {
	public static BossHandler CreateEnemy(Vector3 spawnPosition, PlayerHandler playerHandler) 
    {
        Transform bossTransform = Instantiate(GameAssets.i.pfBoss, spawnPosition, Quaternion.identity);
        BossHandler bossHandler = bossTransform.GetComponent<BossHandler>();

        HealthSystem healthSystem = new HealthSystem((1500 * GameControl.control.lvl));

        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 1.5f), Quaternion.identity, bossTransform).GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);

        bossHandler.Setup(healthSystem);

        return bossHandler;
    }
    private PlayerHandler playerHandler;

	private Vector3 moveDir;
	private Vector3 boss;
	private float speed = 2;
	private bool finished, trigger, shooting;
	public static Quaternion rotation;
	private HealthSystem healthSystem;
	public event EventHandler OnDead;
    public Transform attackPoint;
	public LayerMask whatIsPlayer;
	private bool aoe, iceStarted, aoeStarted;
    private State state;
	private List<Transform> ice;
	private bool firstPhase, secondPhase;
	public static int dmgAoe, dmgCharge, dmgShooting;
	private Animator animator;

	private Coroutine firstPhaseRoutine;
	public Sprite right, upright, up, upleft, left, leftdown, down, rightdown;
	int chargeCount;
	bool waitingCharge, waitingAttack, canAttack;

	private void Setup(HealthSystem healthSystem) 
    {
        this.healthSystem = healthSystem;

        healthSystem.OnDead += HealthSystem_OnDead;
    }

	private enum State 
    {
        Normal,
		Shooting,
		Charging,
		Aoe,
        Busy,
        Dead,
    }

	void Start () 
	{
		state = State.Normal;

		animator = GetComponent<Animator>();
		ice = new List<Transform>();
		firstPhaseRoutine = StartCoroutine(FirstPhase());
		
		dmgCharge = 30;
		dmgAoe = 45;
		dmgShooting = 25;
	}

	void FixedUpdate () 
	{
		if (!firstPhase &&  GetHealthSystem().GetHealthPercent() >= .25f)
			firstPhaseRoutine = StartCoroutine(FirstPhase());
		else if (!secondPhase && GetHealthSystem().GetHealthPercent() < .25f) {
			StopCoroutine(firstPhaseRoutine);
			StartCoroutine(SecondPhase()); 
		}

		if (state == State.Normal)
			this.gameObject.layer = 9;
		else 
			this.gameObject.layer = 2;

		switch (state) {
			case State.Normal:
				HandleMovement();

				gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				transform.rotation = Quaternion.identity;
				chargeCount = 10;

				if (ice.Count > 0) {
					for (int i = 0; i < ice.Count; i++)
						if (ice[i] != null) Destroy(ice[i].gameObject);

					ice.Clear();
				}
				
				break;
			case State.Shooting:
				StartRotating();
				break;
			case State.Charging:
				ChargeAttack();
				break;
			case State.Aoe:
				AoeAttack();
				break;
			case State.Busy:
				gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .2f, .2f, 1);
				if (waitingCharge)
					StartCoroutine(WaitBtwnCharge());
				break;
			default:
				break;
		}
	}

	private IEnumerator FirstPhase()
	{
		firstPhase = true;
		state = State.Normal;
		yield return new WaitForSeconds(2);
		state = State.Charging;
		yield return new WaitForSeconds(20);
		state = State.Normal;
		yield return new WaitForSeconds(5);
		animator.SetBool("shooting", true);
		state = State.Shooting;
		yield return new WaitForSeconds(12);
		animator.SetBool("shooting", false);
		firstPhase = false;
	}
	
	private IEnumerator SecondPhase()
	{
		secondPhase = true;
		state = State.Aoe;
		yield return new WaitForSeconds(41);
		state = State.Normal;
		yield return new WaitForSeconds(4);
		animator.SetBool("shooting", true);	
		state = State.Shooting;
		yield return new WaitForSeconds(14);
		animator.SetBool("shooting", false);
		state = State.Normal;
		yield return new WaitForSeconds(5);
		state = State.Charging;
		yield return new WaitForSeconds(20);
		state = State.Normal;
		yield return new WaitForSeconds(7);
		secondPhase = false;
	}

	private void HandleMovement() {}
	//charge attack
	private void ChargeAttack() 
	{
		this.GetComponent<SpriteRenderer>().sprite = upleft;
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .5f, .5f, 1);
		moveDir = (PlayerHandler.playerHandler.GetPosition() - this.transform.position);
		Vector3 tmpDir = moveDir.normalized;
		Debug.Log(moveDir.normalized);
		if (tmpDir != Vector3.zero) {
            if (tmpDir.y >= 0.5f && tmpDir.x <= -0.5f)
                gameObject.GetComponent<SpriteRenderer>().sprite = upleft;
 
            if (tmpDir.y >= 0.5f && tmpDir.x >= 0.5f)
                gameObject.GetComponent<SpriteRenderer>().sprite = upright;
 
            if (tmpDir.y >= 0.5f && tmpDir.x <= -0.5f)
                gameObject.GetComponent<SpriteRenderer>().sprite = leftdown;
 
            if (tmpDir.y >= 0.5f && tmpDir.x <= 1)
                gameObject.GetComponent<SpriteRenderer>().sprite = rightdown;
        }
 
        else
        {
            //left/right/up/down
            if (tmpDir.x == -1)
                gameObject.GetComponent<SpriteRenderer>().sprite = left;
 
            if (tmpDir.x == 1)
                gameObject.GetComponent<SpriteRenderer>().sprite = right;
 
            if (tmpDir.y == 1)
                gameObject.GetComponent<SpriteRenderer>().sprite = up;
 
            if (tmpDir.y == -1)
                gameObject.GetComponent<SpriteRenderer>().sprite = down;
        }

		if (moveDir.magnitude > 2.3f)
		{
			if (!waitingAttack)
				StartCoroutine(WaitBtwnAttack());
			else if (canAttack)
				transform.position = transform.position + moveDir.normalized * (speed * 6) * Time.deltaTime;
		} 
		else 
		{
			if (state == State.Charging && chargeCount > 0) 
			{
				chargeCount--;
				state = State.Busy;
				waitingAttack = false;
				canAttack = false;
				StartCoroutine(Attack());
			} else {
				state = State.Normal;
			}
		}
	}

	private IEnumerator Attack() 
	{
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .1f, 1, 1);
		yield return new WaitForSeconds(.18f);
		Collider2D player = Physics2D.OverlapCircle(attackPoint.position, 1f, whatIsPlayer);
        if (player != null) {
            PlayerHandler.playerHandler.GetHealthSystem().Damage(dmgCharge);
            float ranDir = UnityEngine.Random.Range(1.5f, 4.5f);
            PlayerHandler.playerHandler.CreateText(Color.red, PlayerHandler.playerHandler.transform.position, new Vector2(-1, ranDir), "-" + 30);
		}

		waitingCharge = true;
	}

	private IEnumerator WaitBtwnAttack()
	{
		waitingAttack = true;
		yield return new WaitForSeconds(.5f);
		canAttack = true;
	}

	private IEnumerator WaitBtwnCharge()
	{	waitingCharge = false;
		yield return new WaitForSeconds(.7f);
		state = State.Charging;
	}
	//end of charge

	//rotating+shooting randomly
	private void StartRotating()
	{
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, .5f, 1);
		float distanceToCenter = Vector3.Distance(EnemySpawnerBoss.roomCenter, transform.position);

		if (distanceToCenter > 1)
		{
			moveDir = (EnemySpawnerBoss.roomCenter - transform.position).normalized;
			transform.position = transform.position + moveDir * speed * Time.deltaTime;
		} else {
			StartCoroutine(RotationAttack());
		}
	}

	private IEnumerator RotationAttack()
	{
		while (!shooting)
			StartCoroutine(Shooting());

		yield return new WaitForSeconds(13);
	}

	private IEnumerator Shooting()
	{
		shooting = true;
		Instantiate(GameAssets.i.pfFireBallBoss, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(.05f);
		shooting = false;
	}
	//end of shooting

	//aoe attack
	private void AoeAttack() 
	{
		gameObject.GetComponent<SpriteRenderer>().color = new Color(.1f, .1f, 1, 1);
	
		if (!aoe)
			StartCoroutine(Aoe());
	}

	private IEnumerator Aoe() 
	{
		aoe = true;
		aoeStarted = true;
		iceStarted = true;
		Transform aoeObj = Instantiate(GameAssets.i.pfCircle, PlayerHandler.playerHandler.GetPosition(), Quaternion.identity);		
		aoeObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(.1f, .1f, 1, .5f);

		if (iceStarted) StartCoroutine(SpawnIce());
		if (aoeStarted) StartCoroutine(EndAoe(40));
		yield return new WaitForSeconds(1.5f);

		aoeObj.GetComponent<ParticleSystem>().Play();
		
		Collider2D player = Physics2D.OverlapCircle(aoeObj.position, 4f, whatIsPlayer);
        if (player != null) {
            PlayerHandler.playerHandler.GetHealthSystem().Damage(dmgCharge);
            float ranDir = UnityEngine.Random.Range(1.5f, 4.5f);
            PlayerHandler.playerHandler.CreateText(Color.red, PlayerHandler.playerHandler.transform.position, new Vector2(-1, ranDir), "-" + 30);
		}

		aoe = false;

		Destroy(aoeObj.gameObject, 1.2f);
	}

	private IEnumerator SpawnIce()
	{
		iceStarted = false;
		Vector3 spawnPosition = PlayerHandler.playerHandler.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(3.5f, 4.5f); 
		ice.Add(Instantiate(GameAssets.i.pfCircleIce, spawnPosition, Quaternion.identity));
		yield return new WaitForSeconds(.2f);
	}

	private IEnumerator EndAoe(float time) {
		aoeStarted = false;
		yield return new WaitForSeconds(time);
	}
	//end of aoe


	public HealthSystem GetHealthSystem() 
    {
        return healthSystem;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) 
    {
        if (OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, 1f);
	}
}