using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	bool isPause;
	[SerializeField] GameObject menu;
	private Button resume;
	private Button town;
	private Button quit;

	void Awake()
	{
		isPause = false;

        menu.SetActive(false);
		resume = menu.transform.GetChild(0).GetComponent<Button>();
        town = menu.transform.GetChild(1).GetComponent<Button>();
        quit = menu.transform.GetChild(2).GetComponent<Button>();
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

		resume.onClick.AddListener(Resume);
		town.onClick.AddListener(BackToTown);
		quit.onClick.AddListener(Quit);
	}

    private void Resume()
    {
        menu.SetActive(false);
		Time.timeScale = 1;
    }

    private void Stop()
    {
        menu.SetActive(true);
		Time.timeScale = 0;
    }

	private void BackToTown()
	{
		SceneManager.LoadScene(0);
	}
	
	private void Quit()
	{
		#if UNITY_EDITOR
			Debug.Log("Quit!");
		#endif
		Application.Quit();
	}
}
