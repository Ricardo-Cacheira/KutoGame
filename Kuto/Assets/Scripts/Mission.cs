using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Mission : MonoBehaviour , IPointerClickHandler{

	public void OnPointerClick(PointerEventData eventData)
	{
		SceneManager.LoadScene("Prototype");
	}
}
