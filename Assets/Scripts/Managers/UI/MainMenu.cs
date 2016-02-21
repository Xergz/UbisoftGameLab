using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public int sceneToLoad = 0;
    public int mainMenuScene = 0;
    public GameObject mainMenu;
    public GameObject eventSystem;

    void Awake() {
#if UNITY_EDITOR
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
#endif
    }

    void OnLevelWasLoaded(int level) {
        if (level == mainMenuScene) {
            mainMenu.SetActive(true);
            eventSystem.SetActive(true);
            eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(mainMenu.transform.Find("Play").gameObject);
        } else {
            mainMenu.SetActive(false);
        }   
    }

    public void Play () {
        SceneManager.LoadScene(sceneToLoad);
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
