using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Backend.Core;
using System.Text;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	public string mainSceneName;

	public EventSystem eventSystem;

	public Button continueButton;
	public Button newGameButton;
	public Button quitButton;


	void Start() {
#if UNITY_EDITOR
		OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
#endif
	}

	public void OnLevelWasLoaded(int level) {
		if (level == SceneManager.GetSceneByName("mainMenu").buildIndex) {
			gameObject.SetActive(true);
			eventSystem.gameObject.SetActive(true);

			if(GameManager.CountSavedCheckpoints() != 0) {
				eventSystem.firstSelectedGameObject = continueButton.gameObject;
				eventSystem.SetSelectedGameObject(continueButton.gameObject);
				continueButton.interactable = true;

				Navigation newNav = newGameButton.navigation;
				newNav.selectOnDown = continueButton;
				newGameButton.navigation = newNav;

				newNav = quitButton.navigation;
				newNav.selectOnUp = continueButton;
				quitButton.navigation = newNav;
			} else {
				eventSystem.firstSelectedGameObject = newGameButton.gameObject;
				eventSystem.SetSelectedGameObject(newGameButton.gameObject);
				continueButton.interactable = false;

				Navigation newNav = newGameButton.navigation;
				newNav.selectOnDown = quitButton;
				newGameButton.navigation = newNav;

				newNav = quitButton.navigation;
				newNav.selectOnUp = newGameButton;
				quitButton.navigation = newNav;
			}
		} else if (level == SceneManager.GetSceneByName("Extended").buildIndex) {
			gameObject.SetActive(false);
			eventSystem.gameObject.SetActive(false);
		} else {
            gameObject.SetActive(false);
        }
	}

	public void Continue() {
		// Transition out of main menu
		// Fade to black
		SceneManager.LoadScene(mainSceneName);

		GameManager.RestoreFromLastCheckpoint();
#if UNITY_EDITOR
		UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(mainSceneName).buildIndex);
#endif

		// Fade to game
	}

	public void NewGame () {
		GameManager.DeleteAllCheckPoints();
		// Transition out of main menu
		// Fade to black
		SceneManager.LoadScene(mainSceneName);

#if UNITY_EDITOR
		UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(mainSceneName).buildIndex);
#endif
		PlayerController.HasWon = false;
		// Fade to game
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
