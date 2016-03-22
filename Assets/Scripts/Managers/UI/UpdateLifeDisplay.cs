using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateLifeDisplay : MonoBehaviour {

    public Text count;

	// Use this for initialization
	void Start () {
        if (count == null)
            count = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        count.text = PlayerController.GetCollectedFragments().Count.ToString();
	}
}
