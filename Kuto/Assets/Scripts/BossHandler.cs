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

        HealthBar healthBar = Instantiate(GameAssets.i.pfHealthBar, spawnPosition + new Vector3(0, 3.5f), Quaternion.identity, bossTransform).GetComponent<HealthBar>();
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

	private Coroutine firstPhaseRoutine;

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
		yield return new WaitForSeconds(7);
		state = State.Charging;
		yield return new WaitForSeconds(20);
		state = State.Normal;
		yield return new WaitForSeconds(10);
		state = State.Shooting;
		yield return new WaitForSeconds(10);
		firstPhase = false;
	}
	
	private IEnumerator SecondPhase()
	{
		secondPhase = true;
		state = State.Aoe;
		yield return new WaitForSeconds(41);
		state = State.Normal;
		yield return new WaitForSeconds(7);
		state = State.Shooting;
		yield return new WaitForSeconds(14);
		state = State.Normal;
		yield return new WaitForSeconds(7);
		state = State.Charging;
		yield return new WaitForSeconds(20);
		state = State.Normal;
		yield return new WaitForSeconds(5);
		secondPhase = false;
	}

	private void HandleMovement()
	{
		if (!trigger) {
			transform.position = new Vector3(transform.position.x + .1f, transform.position.y, transform.position.z);
			if (transform.position.x > 5) trigger = true;
		} else if (trigger)
		{
			transform.position = new Vector3(transform.position.x - .1f, transform.position.y, transform.position.z);
			if (transform.position.x < -5) trigger = false;
		}
	}

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
		transform.Rotate(new Vector3(0, 0, 6));

		while (!shooting)
			StartCoroutine(Shooting());

		yield return new WaitForSeconds(13);
	}

	private IEnumerator Shooting()
	{
		shooting = true;
		Instantiate(GameAssets.i.pfFireBallBoss, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(.07f);
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

	//charge attack
	private void ChargeAttack() 
	{
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .5f, .5f, 1);
		moveDir = (PlayerHandler.playerHandler.GetPosition() - this.transform.position);

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
		Vector3 difference = PlayerHandler.playerHandler.GetPosition() - transform.position;
 		float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + 90;
		yield return new WaitForSeconds(.18f);
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
		Collider2D player = Physics2D.OverlapCircle(attackPoint.position, 1f, whatIsPlayer);
        if (player != null) {
            PlayerHandler.playerHandler.GetHealthSystem().Damage(dmgCharge);
            float ranDir = UnityEngine.Random.Range(1.5f, 4.5f);
            PlayerHandler.playerHandler.CreateText(Color.red, PlayerHandler.playerHandler.transform.position, new Vector2(-1, ranDir), "-" + 30);
		}

		if (PlayerHandler.playerHandler.GetHealthSystem().GetHealthPercent() <= 0) {
            GameHandler.Restart();
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