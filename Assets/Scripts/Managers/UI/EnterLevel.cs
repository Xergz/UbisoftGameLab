using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnterLevel : MonoBehaviour {

    public FadeInOut iTween;
    public float titleDisplayTime = 3f;
    public Text title;

    private float initialTime = -1f;

	// Use this for initialization
	private void Start () {
        if (iTween == null)
            iTween = GetComponent<FadeInOut>();
	}

    public void DisplayLevel(string levelName) {
        title.text = levelName;
        initialTime = Time.time + iTween.fadeInTime;
        iTween.FadeIn();
    }
	
	// Update is called once per frame
	private void Update () {
        if (initialTime > 0 && Time.time - initialTime > titleDisplayTime) {
            initialTime = -1f;
            iTween.FadeOut();
        }
	}
}
