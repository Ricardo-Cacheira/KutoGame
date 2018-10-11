using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	Rigidbody2D rigidbody2D;
	Transform transform;
	Vector2 movement;

	bool dashing;
	public float speed = 5f;
	public float dashSpeed = 10f;
	public float dashTime = 0.3f;
	public float recoveryTime = 0.5f;
	private float timeStamp;
	private float lastX;
	private float lastY;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		transform = GetComponent<Transform>();
		dashing = false;
		movement = Vector2.zero;
		timeStamp = Time.time;
	}
	

	void Update()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		float lookx = Input.GetAxisRaw("HorizontalLook");
		float looky = Input.GetAxisRaw("VerticalLook");

		
		// Debug.Log("lookx: " + lookx + " looky: " + looky);
		
		if (dashing)
		{
			StartCoroutine(Dash());
			
		}else
		{
			movement = new Vector2(x, y);
			rigidbody2D.velocity = movement * speed;

				// transform.eulerAngles = new Vector3(0,0, Mathf.Atan2(lookx,looky)*180 / Mathf.PI);
			if (lookx != 0 || looky != 0)
			{
			}

			if(Input.GetButtonDown("Dash") && timeStamp <= Time.time)
			{
				dashing = true;
				Debug.Log("dashing");
			}
		}

		if(movement!= Vector2.zero)
		{
			lastX = x;
			lastY = y;
		}
	}

	IEnumerator Dash()
	{
		if(movement == Vector2.zero)
		{
			movement = new Vector2(lastX, lastY);
			Debug.Log("last");
		}

		timeStamp = Time.time + recoveryTime;
		rigidbody2D.velocity = movement * dashSpeed;
		yield return new WaitForSeconds(dashTime);
		dashing = false;
	}
}
