using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class InputHandler : MonoBehaviour {

	public void BasicAttack()
	{
		PlayerHandler.phoneAttack = true;
	}

	public void Shooting()
	{
		PlayerHandler.phoneShooting = true;
	}

	public void Aoe()
	{
		PlayerHandler.phoneAoe = true;

	}

	public void Heal()
	{
		PlayerHandler.phoneHeal = true;

	}

	public void Dash()
	{
		PlayerHandler.phoneDash = true;

	}
}
