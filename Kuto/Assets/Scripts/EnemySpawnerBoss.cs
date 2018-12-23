using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawnerBoss : MonoBehaviour {

	GameObject cameraFollow;
	CameraShaker cameraShaker;
	Transform boss;
	bool started, spawning;
	public static Vector3 roomCenter;
	GameObject gameHandle;
	GameHandler gameHandler;
	public GameObject door;

	void Start () {
		gameHandle = GameObject.Find("GameManager");
		gameHandler = gameHandle.GetComponent<GameHandler>();

		cameraFollow = GameObject.Find("CameraFollow");
		cameraShaker = cameraFollow.GetComponent<CameraShaker>();

		roomCenter = new Vector3(transform.position.x, transform.position.y);

	}
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!started)
			StartCoroutine(ShakeScreen());
	}

	private IEnumerator ShakeScreen()
	{
		FindObjectOfType<AudioManager>().Play("Rumble");
		door.GetComponent<TilemapRenderer>().enabled = true;
		door.GetComponent<TilemapCollider2D>().enabled = true;
		started = true;
		cameraFollow.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
		cameraShaker.shakeDuration = 1f;
		cameraShaker.enabled = true;
		PlayerHandler.playerHandler.speed = 0;
		yield return new WaitForSeconds(1);
		gameHandler.SpawnBoss();
		cameraShaker.shakeAmount = 0;
		yield return new WaitForSeconds(3);
		PlayerHandler.playerHandler.speed = 6f;
		cameraShaker.enabled = false;
		Destroy(gameObject);
	}
}
