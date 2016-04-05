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

    public void LoadLevel(string name, bool loadCheckpoint = false, bool saveCheckpoint = false, string checkpointName = "") {
        SceneManager.LoadScene(name);

        if (loadCheckpoint)
            StartCoroutine(LoadCheckpoint());

		if(saveCheckpoint)
			StartCoroutine(SaveCheckpoint(checkpointName));

		UIManager.instance.CallOnLevelWasLoaded(SceneManager.GetSceneByName(name).buildIndex);
    }

    IEnumerator LoadCheckpoint() {
        yield return null;
        GameManager.RestoreFromLastCheckpoint();
    }

	IEnumerator SaveCheckpoint(string checkpointName) {
		yield return null;
		GameManager.SaveCheckpoint(new Checkpoint(checkpointName));
	}
}
