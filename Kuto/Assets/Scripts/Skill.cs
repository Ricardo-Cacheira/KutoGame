using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetingMode {
	Cone,
	Single, //Single target / projectile
	AroundAoe,
}

public enum Effect {
	Damage,
	Stun,
	Pushback, //KnockBack
	Pull
}


public class Skill : MonoBehaviour {

	public TargetingMode targetingMode;
	public TargetingMode Effect;
	public int speed;
	
	[Space] // Visible in unity editor

	public int damage;
	public int stun;
	public int pushback;
	public int pull;

	[Space] // Visible in unity editor
	
	public int damagePercentage;
	public int stunPercentage;
	public int pushbackPercentage;
	public int pullPercentage;
}
