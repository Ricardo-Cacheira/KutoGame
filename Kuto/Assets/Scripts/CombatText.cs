using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {
	public Text combatText;
    public float guiTime = 1.5f;

	private float speed;
	private Vector3 direction;
	private float fadeTime;


	public void Initialize(float speed, string damageMessage, Vector3 direction)
	{
		this.speed = speed;
		this.direction = direction;
		combatText.text = damageMessage;
		StartCoroutine(FadeOut());
	}
	void Update()
	{
		float translation = speed * Time.deltaTime;
		transform.Translate(direction * translation);
	}

	IEnumerator FadeOut()
    {
		float startAlpha = combatText.GetComponent<Text>().color.a;

		float rate = 1.0f / guiTime;
		float progress = 0.0f;

		while (progress < 1.0)
		{
			Color tmpColor = GetComponent<Text>().color;
			combatText.GetComponent<Text>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
		
			progress += rate * Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);

    }
}
