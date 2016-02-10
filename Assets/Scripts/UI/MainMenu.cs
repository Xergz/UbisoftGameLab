using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public int SceneToLoad = 0;

	public void Play () {
        SceneManager.LoadScene(SceneToLoad);
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
