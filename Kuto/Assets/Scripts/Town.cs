using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour {

	public Camera camera;

	void ResizeSpriteToScreen() {
     var sr = GetComponent<SpriteRenderer>();
     if (sr == null) return;
     
     transform.localScale = new Vector3(1,1,1);
     
     float width = sr.sprite.bounds.size.x;
     float height = sr.sprite.bounds.size.y;
     
     float worldScreenHeight = camera.orthographicSize * 2.0f;
     float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

     transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height);
 	}
}
