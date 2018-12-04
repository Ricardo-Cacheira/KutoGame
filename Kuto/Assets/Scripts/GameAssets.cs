using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAssets : MonoBehaviour {
    public static GameAssets i;
    
    private void Awake() 
    {
        i = this;
    }

    public Image potion;
    public Image aoeFire;
    public Image bullet;
    public Transform pfPlayerTransform;
    public Transform pfEnemyTransform;
    public Transform pfEnemyRangedTransform;
    public Transform pfEnemySlowerTransform;
    public Transform pfHealthBar;
    public Transform pfXpBar;
    public Transform pfFireBall;
    public Transform pfFireBallBoss;
    public Transform pfVoidBall;
    public Transform pfSlowWave;
    public Transform pfCircle;
    public Transform pfCircleIce;
    public Transform pfBomb;
    public Transform pfMissile;  
    public RectTransform combatText;
    public Transform pfBoss;
}