using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceSystem {

	public event EventHandler OnXpChanged;
	private int xpMax;
	private int gameplayXp, tempXp, tempCurrXp;
	public bool xpIsMax;

	public ExperienceSystem(int xpMax) {
		this.xpMax = xpMax;
	}
	
	public float GetXpPercent() {
		gameplayXp = GameControl.control.xp;
		tempXp = GameControl.control.tempXp;
		tempCurrXp = GameControl.control.currReqXp;
		float tempPercent = (float) (gameplayXp - tempXp) / (tempCurrXp) + 1;

		if (float.IsInfinity(tempPercent)) 
			return 1;
		else if (tempPercent < 0)
			return 0;
		else
			return tempPercent;
	}

	public void WinXp(int amount) {
		GameControl.control.xp += amount;
		if (GameControl.control.xp > xpMax) {
			PlayerHandler.playerHandler.xp = xpMax;
			GameControl.control.xp = xpMax;
			GameControl.control.isXpMax = true;
		}
		if (OnXpChanged != null) OnXpChanged(this, EventArgs.Empty);
	}
}
