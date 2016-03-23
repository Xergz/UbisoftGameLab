using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoading : MonoBehaviour {

    public static LevelLoading instance = null;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string name, bool loadCheckpoint = false) {
        SceneManager.LoadScene(name);

        if (loadCheckpoint)
            StartCoroutine(LoadCheckpoint());
#if UNITY_EDITOR
        UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(name).buildIndex);
#endif
    }

    IEnumerator LoadCheckpoint() {
        yield return null;
        GameManager.RestoreFromLastCheckpoint();
    }
}
