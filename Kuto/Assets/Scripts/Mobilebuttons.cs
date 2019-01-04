using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mobilebuttons : MonoBehaviour {

	public Sprite aoe, potion, bullet;
	public Image k,l,i;
	

	void Start () {
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
                    //stun
                    break;
                default: break;
            }

        }
		// k.targetGraphic = newImage;
	}
	
	
}
