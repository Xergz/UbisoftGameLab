using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour {
	public string mainMenuSceneName;
    public string gameSceneName;

	public EventSystem eventSystem;

	public Button checkpointButton;
	public Button mainMenuButton;


	private bool onPause = false;

	void Start() {
#if UNITY_EDITOR
		OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
#endif
	}

	public void OnLevelWasLoaded(int level) {
        if (level == SceneManager.GetSceneByName("gameOver").buildIndex) {
            gameObject.SetActive(true);
            eventSystem.gameObject.SetActive(true);
            eventSystem.firstSelectedGameObject = mainMenuButton.gameObject;
            eventSystem.SetSelectedGameObject(mainMenuButton.gameObject);

            if (GameManager.CountSavedCheckpoints() != 0) {
                checkpointButton.interactable = true;

                Navigation newNav = mainMenuButton.navigation;
                newNav.selectOnDown = checkpointButton;
                mainMenuButton.navigation = newNav;
            } else {
                checkpointButton.interactable = false;

                Navigation newNav = mainMenuButton.navigation;
                newNav.selectOnDown = mainMenuButton;
                mainMenuButton.navigation = newNav;
            }
        } else {
            gameObject.SetActive(false);
        }
    }	

	public void ReloadLastCheckpoint() {
        SceneManager.LoadScene(gameSceneName);
        GameManager.RestoreFromLastCheckpoint();

#if UNITY_EDITOR
        UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(gameSceneName).buildIndex);
#endif
    }

    public void MainMenu() {
		SceneManager.LoadScene(mainMenuSceneName);

#if UNITY_EDITOR
		UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(mainMenuSceneName).buildIndex);
#endif
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
