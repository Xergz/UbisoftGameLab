using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	public string mainMenuSceneName;

	public EventSystem eventSystem;

	public Button resumeButton;
	public Button checkpointButton;
	public Button mainMenuButton;


	public bool onPause = false;

	void Start() {
		//LevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
	}

	public void LevelWasLoaded(int level) {
		gameObject.SetActive(false);
	}

	public void Pause() {
		if(SceneManager.GetActiveScene().name != "mainMenu") {
			onPause = !onPause;

			if(onPause) {
				gameObject.SetActive(true);
				eventSystem.gameObject.SetActive(true);
				eventSystem.firstSelectedGameObject = resumeButton.gameObject;
				eventSystem.SetSelectedGameObject(resumeButton.gameObject);
				Time.timeScale = 0;

				if(GameManager.CountSavedCheckpoints() != 0) {
					checkpointButton.interactable = true;

					Navigation newNav = resumeButton.navigation;
					newNav.selectOnUp = checkpointButton;
					resumeButton.navigation = newNav;

					newNav = mainMenuButton.navigation;
					newNav.selectOnDown = checkpointButton;
					mainMenuButton.navigation = newNav;
				} else {
					checkpointButton.interactable = false;

					Navigation newNav = resumeButton.navigation;
					newNav.selectOnUp = mainMenuButton;
					resumeButton.navigation = newNav;

					newNav = mainMenuButton.navigation;
					newNav.selectOnDown = resumeButton;
					mainMenuButton.navigation = newNav;
				}
			} else {
				eventSystem.gameObject.SetActive(false);
				gameObject.SetActive(false);
				Time.timeScale = 1;
			}
		}
	}

	public void ReloadLastCheckpoint() {
		GameManager.RestoreFromLastCheckpoint();
		Pause();
	}

	public void MainMenu() {
		Time.timeScale = 1;
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
