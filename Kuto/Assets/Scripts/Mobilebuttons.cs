using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mobilebuttons : MonoBehaviour {

	private Sprite aoe, potion, bullet, stun;
	public Image k,l,i;
	

	void Start () {

        aoe = GameAssets.i.aoeFire.sprite;
        potion = GameAssets.i.potion.sprite;
        bullet = GameAssets.i.bullet.sprite;
        stun = GameAssets.i.stun.sprite;

		Image[] buttons = new Image[] { k,l,i };
		for (int i = 0; i < GameControl.control.cooldowns.Length; i++)
        {
            switch(GameControl.control.cooldowns[i])
            {
                case 0:
					buttons[i].sprite = null; 
                    break;
                case 1:
                    buttons[i].sprite = potion; 
                    break;
                case 2:
                    buttons[i].sprite = aoe; 
                    break;
                case 3:
                    buttons[i].sprite = bullet; 
                    break;
                case 4:
                    buttons[i].sprite = stun; 
                    break;
                default: break;
            }

        }
		// k.targetGraphic = newImage;
	}
	
	
}
