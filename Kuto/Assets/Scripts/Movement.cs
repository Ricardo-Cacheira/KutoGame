using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	private Rigidbody2D rigidbody2D;

	Vector2 movement;

	bool dashing, leaping;
	public float speed = 5f;
	public float dashSpeed = 10f;
	public float dashTime = 0.3f;
	public float recoveryTime = 0.5f;
	private float timeStamp;
	public float lastX = 1f;
	public float lastY = 0f;

	void Start () {
		
		rigidbody2D = GetComponent<Rigidbody2D>();
		dashing = false;
		movement = Vector2.zero;
		timeStamp = Time.time;
	}
	

	void FixedUpdate()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		float lookx = Input.GetAxisRaw("HorizontalLook");
		float looky = Input.GetAxisRaw("VerticalLook");
		
		if (dashing)
		{
			StartCoroutine(Dash());
			
		}else if(leaping)
		{
			StartCoroutine(Leap());
		}else
		{
			movement = new Vector2(x, y);
			rigidbody2D.velocity = movement * speed;

			if(movement == Vector2.zero)
			{
				movement = new Vector2(lastX, lastY).normalized;
				Debug.Log(new Vector2(lastX, lastY));
			}else if(movement != new Vector2(lastX,lastY)){
			}
				// transform.eulerAngles = new Vector3(0,0, Mathf.Atan2(lookx,looky)*180 / Mathf.PI);
			if (lookx != 0 || looky != 0)
			{
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
		}

		if(movement!= Vector2.zero && !dashing && !leaping) 
		{
			lastX = x;
			lastY = y;
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
		rigidbody2D.velocity = (movement * -1) * dashSpeed*1.5f;
		yield return new WaitForSeconds(dashTime);
		leaping = false;
	}
}
