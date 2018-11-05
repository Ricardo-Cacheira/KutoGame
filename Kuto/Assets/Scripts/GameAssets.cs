using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAssets : MonoBehaviour {
    public static GameAssets i;
    
    private void Awake() {
        i = this;
    }
    public Image potion;
    public Image aoeFire;
    public Image  bullet;
    public Transform pfPlayerTransform;
    public Transform pfEnemyTransform;
    public Transform pfEnemyRangedTransform;
    public Transform pfHealthBar;
    public Transform pfFireBall;
    public Transform pfVoidBall;
    public Transform pfCircle;
    
}