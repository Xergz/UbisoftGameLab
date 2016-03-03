using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public int mainMenuScene = 0;
    public GameObject pauseMenu;
    public GameObject eventSystem;

    public Button checkpointButton;
    public GameManager GameManager;

    private bool onPause = false;

    void Awake() {
#if UNITY_EDITOR
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
#endif
    }

    void OnLevelWasLoaded(int level) {
        pauseMenu.SetActive(false);
        if (level != mainMenuScene) {
            eventSystem.SetActive(false);
            eventSystem.GetComponent<EventSystem>().firstSelectedGameObject = pauseMenu.transform.Find("Resume").gameObject;
        }
    }

    public void Pause() {
        if(SceneManager.GetActiveScene().buildIndex != mainMenuScene) {
            onPause = !onPause;

            if (onPause) {
                eventSystem.SetActive(true);
                pauseMenu.SetActive(true);
                Time.timeScale = 0;

                if(GameManager.CountSavedCheckpoints() == 0) {
                    checkpointButton.interactable = false;
                }

            } else {
                eventSystem.SetActive(false);
                pauseMenu.SetActive(false);
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
        SceneManager.LoadScene(mainMenuScene);
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
