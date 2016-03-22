using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoading : MonoBehaviour {

    public void LoadEndLevel(string name) {
        SceneManager.LoadScene(name);
#if UNITY_EDITOR
        UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(name).buildIndex);
#endif
    }
}
