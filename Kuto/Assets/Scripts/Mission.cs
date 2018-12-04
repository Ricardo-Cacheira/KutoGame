using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Mission : MonoBehaviour , IPointerClickHandler{

	public void OnPointerClick(PointerEventData eventData)
	{
		if (Menu.menu.isTemple)
			SceneManager.LoadScene("Prototype");
		if (Menu.menu.isBeach)
			SceneManager.LoadScene("BeachScene");
	}
}
