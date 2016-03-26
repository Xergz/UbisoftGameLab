using UnityEngine;
using System.Collections;

public class UITriggerHighlight : InputReceiver {

	public GameObject highlightLeft;
	public GameObject highlightRight;

	// Use this for initialization
	void Start() {
		highlightLeft.SetActive(false);
		highlightRight.SetActive(false);
	}

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		switch(inputEvent.InputAxis) {
			case EnumAxis.LeftTrigger:
				if(inputEvent.Value > 0.01)
					highlightLeft.SetActive(true);
				else
					highlightLeft.SetActive(false);
				break;
			case EnumAxis.RightTrigger:
				if(inputEvent.Value > 0.01) 
					highlightRight.SetActive(true);
				else
					highlightRight.SetActive(false);
				break;
			default:
				break;
		}
	}
}
