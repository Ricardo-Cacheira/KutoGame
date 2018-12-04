using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceBar : MonoBehaviour {

	private ExperienceSystem xpSystem;

	public void Setup(ExperienceSystem xpSystem) {
		this.xpSystem = xpSystem;

		xpSystem.OnXpChanged += XpSystem_OnXpChanged;
		UpdateXpBar();
	}

	private void XpSystem_OnXpChanged(object sender, System.EventArgs e) {
		UpdateXpBar();
	}
	private void UpdateXpBar() {
		transform.Find("BarXp").localScale = new Vector3(xpSystem.GetXpPercent(), 1);
	}
}
