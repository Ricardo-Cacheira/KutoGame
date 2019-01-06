using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	bool isPause;
	[SerializeField] GameObject menu;

	void Awake()
	{
		isPause = false;

        menu.SetActive(false);
	}

	void Update () {
		if(Input.GetButtonDown("Cancel") && !isPause)
		{
			isPause = true;
			Stop();
		}else if(Input.GetButtonDown("Cancel"))
		{
			isPause = false;
			Resume();
		}
	}

    public void Resume()
    {
        menu.SetActive(false);
		Time.timeScale = 1;
    }

    private void Stop()
    {
        menu.SetActive(true);
		Time.timeScale = 0;
    }

	public void BackToTown()
	{
		SceneManager.LoadScene(1);
	}
	
	public void Quit()
	{
		#if UNITY_EDITOR
			Debug.Log("Quit!");
		#endif
		Application.Quit();
	}
}
