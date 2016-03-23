using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public InputReceiver cameraController;
	public InputReceiver playerController;
	public InputReceiver streamController;
	public InputReceiver uiController;
    public InputReceiver uiInGameButtons;

	public float timeBeforeHeldDown = 0.15F;


	private float LeftJoystickXStatus;
	private float LeftJoystickYStatus;
	private float RightJoystickXStatus;
	private float RightJoystickYStatus;
	private float LeftTriggerStatus;
	private float RightTriggerStatus;
	private float AButtonTimeAtDown = 0F;
	private float BButtonTimeAtDown = 0F;
	private float XButtonTimeAtDown = 0F;
	private float YButtonTimeAtDown = 0F;

	private bool AButtonHeldDown = false;
	private bool BButtonHeldDown = false;
	private bool XButtonHeldDown = false;
	private bool YButtonHeldDown = false;

	void Awake() {
		cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
		streamController = GameObject.Find("StreamController").GetComponent<StreamController>();
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		GameObject uiManager = GameObject.Find("UIManager");
		if(uiManager == null) {
			uiController = null;
		} else {
			uiController = uiManager.GetComponent<UIManager>();
		}
	}

	void Start() {
		LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
		LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
		RightJoystickXStatus = Input.GetAxis("RightJoystickX");
		RightJoystickYStatus = Input.GetAxis("RightJoystickY");
		LeftTriggerStatus = Input.GetAxis("LeftTrigger");
		RightTriggerStatus = Input.GetAxis("RightTrigger");
	}


	// Update is called once per frame
	void Update() {
		CheckButtons();
		CheckJoysticks();
		CheckTriggers();
		CheckBumpers();
	}

	private void CheckButtons() {
		if(Input.GetButtonDown("AButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float) EnumButtonState.PRESSED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float)EnumButtonState.PRESSED));
            AButtonTimeAtDown = Time.time;
		} else if(Input.GetButtonDown("BButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float) EnumButtonState.PRESSED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float)EnumButtonState.PRESSED));
            BButtonTimeAtDown = Time.time;
		} else if(Input.GetButtonDown("XButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float) EnumButtonState.PRESSED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float)EnumButtonState.PRESSED));
            XButtonTimeAtDown = Time.time;
		} else if(Input.GetButtonDown("YButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float) EnumButtonState.PRESSED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float)EnumButtonState.PRESSED));
            YButtonTimeAtDown = Time.time;
		}

		if(Input.GetButton("AButton")) {
			if(Time.time - AButtonTimeAtDown > timeBeforeHeldDown && !AButtonHeldDown) {
				streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float) EnumButtonState.HELD_DOWN));
                uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float)EnumButtonState.HELD_DOWN));
                AButtonHeldDown = true;
			}
		} else if(Input.GetButton("BButton")) {
			if(Time.time - BButtonTimeAtDown > timeBeforeHeldDown && !BButtonHeldDown) {
				streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float) EnumButtonState.HELD_DOWN));
                uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float)EnumButtonState.HELD_DOWN));
                BButtonHeldDown = true;
			}
		} else if(Input.GetButton("XButton")) {
			if(Time.time - XButtonTimeAtDown > timeBeforeHeldDown && !XButtonHeldDown) {
				streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float) EnumButtonState.HELD_DOWN));
                uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float)EnumButtonState.HELD_DOWN));
                XButtonHeldDown = true;
			}
		} else if(Input.GetButton("YButton")) {
			if(Time.time - YButtonTimeAtDown > timeBeforeHeldDown && !YButtonHeldDown) {
				streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float) EnumButtonState.HELD_DOWN));
                uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float)EnumButtonState.HELD_DOWN));
                YButtonHeldDown = true;
			}
		}

		if(Input.GetButtonUp("AButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (AButtonHeldDown) ?
																				(float) EnumButtonState.RELEASED :
																				(float) EnumButtonState.CLICKED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (AButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            AButtonHeldDown = false;
		} else if(Input.GetButtonUp("BButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (BButtonHeldDown) ?
																				(float) EnumButtonState.RELEASED :
																				(float) EnumButtonState.CLICKED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (BButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            BButtonHeldDown = false;
		} else if(Input.GetButtonUp("XButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (XButtonHeldDown) ?
																				(float) EnumButtonState.RELEASED :
																				(float) EnumButtonState.CLICKED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (XButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            XButtonHeldDown = false;
		} else if(Input.GetButtonUp("YButton")) {
			streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (YButtonHeldDown) ?
																				(float) EnumButtonState.RELEASED :
																				(float) EnumButtonState.CLICKED));
            uiInGameButtons.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (YButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            YButtonHeldDown = false;
		}

		if(Input.GetButtonDown("StartButton")) {
			if(uiController != null)
				uiController.ReceiveInputEvent(new InputEvent(EnumAxis.StartButton, 1));
		}

		if(Input.GetButtonDown("SelectButton")) {
		}
	}

	private void CheckJoysticks() {
		if(Input.GetJoystickNames().Length != 0 && Input.GetJoystickNames()[0] != "") {
			if(Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus) {
				LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
				playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, LeftJoystickXStatus));
			}

			if(Input.GetAxis("LeftJoystickY") != LeftJoystickYStatus) {
				LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
				playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
			}

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            RightJoystickXStatus = Input.GetAxis("RightJoystickXMac");
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
#else
			RightJoystickXStatus = Input.GetAxis("RightJoystickX");
			cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
#endif


#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (Input.GetAxis("RightJoystickYMac") != RightJoystickYStatus) {
                RightJoystickYStatus = Input.GetAxis("RightJoystickYMac");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
            }
#else
			if(Input.GetAxis("RightJoystickY") != RightJoystickYStatus) {
				RightJoystickYStatus = Input.GetAxis("RightJoystickY");
				cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
			}
#endif
		} else {
			if(Input.GetAxis("KeyboardAD") != LeftJoystickXStatus) {
				LeftJoystickXStatus = Input.GetAxis("KeyboardAD");
				playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, LeftJoystickXStatus));
			}

			if(Input.GetAxis("KeyboardWS") != LeftJoystickYStatus) {
				LeftJoystickYStatus = Input.GetAxis("KeyboardWS");
				playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
			}

			RightJoystickXStatus = Input.GetAxis("KeyboardLeftRight");
			cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));

			if(Input.GetAxis("KeyboardUpDown") != RightJoystickYStatus) {
				RightJoystickYStatus = Input.GetAxis("KeyboardUpDown");
				cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
			}
		}

		if(Input.GetButtonDown("LeftJoystickButton")) {
		}

		if(Input.GetButtonDown("RightJoystickButton")) {
			cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, (float) EnumButtonState.PRESSED));
		}

		if(Input.GetButtonUp("RightJoystickButton")) {
			cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, (float) EnumButtonState.RELEASED));
		}
	}

	private void CheckTriggers() {
		if(Input.GetAxis("LeftTrigger") != LeftTriggerStatus) {
			LeftTriggerStatus = Input.GetAxis("LeftTrigger");
			playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftTrigger, LeftTriggerStatus));
		}

		if(Input.GetAxis("RightTrigger") != RightTriggerStatus) {
			RightTriggerStatus = Input.GetAxis("RightTrigger");
			playerController.ReceiveInputEvent(new InputEvent(EnumAxis.RightTrigger, RightTriggerStatus));
		}
	}

	private void CheckBumpers() {
		if(Input.GetButtonDown("LeftBumper")) {
		}
		if(Input.GetButtonDown("RightBumper")) {
		}
	}
}
