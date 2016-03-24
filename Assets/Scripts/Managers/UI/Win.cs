using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class Win : MonoBehaviour {
	public string mainMenuSceneName;

	public EventSystem eventSystem;

    public Button mainMenuButton;

	void Start() {
		OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnLevelWasLoaded(int level) {
        if (level == SceneManager.GetSceneByName("win").buildIndex) {
            gameObject.SetActive(true);
            eventSystem.gameObject.SetActive(true);
            eventSystem.firstSelectedGameObject = mainMenuButton.gameObject;
            eventSystem.SetSelectedGameObject(mainMenuButton.gameObject);
        } else {
            gameObject.SetActive(false);
        }
    }	

	public void KeepPlaying() {
        LevelLoading.instance.LoadLevel("Extended", true);

        UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(mainMenuSceneName).buildIndex);
	}

	public void MainMenu() {
		SceneManager.LoadScene(mainMenuSceneName);

		UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(mainMenuSceneName).buildIndex);
	}

	public void Quit() {
#if UNITY_STANDALONE
		Application.Quit();
#endif

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
