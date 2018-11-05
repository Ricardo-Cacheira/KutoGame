using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Mission : MonoBehaviour , IPointerClickHandler{

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		SceneManager.LoadScene("Prototype");
	}
}
