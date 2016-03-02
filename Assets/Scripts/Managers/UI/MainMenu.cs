using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public int sceneToLoad = 0;
    public int mainMenuScene = 0;
    public GameObject mainMenu;
    public GameObject eventSystem;
    public GameManager GameManager;

    void Awake() {
#if UNITY_EDITOR
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
#endif
    }

    void OnLevelWasLoaded(int level) {
        if (level == mainMenuScene) {
            mainMenu.SetActive(true);
            eventSystem.SetActive(true);
        } else {
            mainMenu.SetActive(false);
        }   
    }

    public void Continue() {
        SceneManager.LoadScene(sceneToLoad);
        GameManager.RestoreFromLastCheckpoint();
    }

    public void NewGame () {
        GameManager.DeleteAllCheckPoints();
        SceneManager.LoadScene(sceneToLoad);
        GameManager.SaveCheckpoint(new Checkpoint());
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
