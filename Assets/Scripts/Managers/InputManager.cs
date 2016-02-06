using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public InputReceiver cameraController;
    public InputReceiver playerController;
    public InputReceiver streamController;

    private float LeftJoystickXStatus;
    private float LeftJoystickYStatus;
    private float RightJoystickXStatus;
    private float RightJoystickYStatus;
    private float LeftTriggerStatus;
    private float RightTriggerStatus;

    void Awake () {
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
    }

    void Start () {
        LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
        LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
        RightJoystickXStatus = Input.GetAxis("RightJoystickX");
        RightJoystickYStatus = Input.GetAxis("RightJoystickY"); ;
        LeftTriggerStatus = Input.GetAxis("LeftTrigger");
        RightTriggerStatus = Input.GetAxis("RightTrigger");
    }


    // Update is called once per frame
    void Update () {
        CheckButtons();
        CheckJoysticks();
        CheckTriggers();
        CheckBumpers();
	}

    private void CheckButtons()
    {
        if (Input.GetButtonDown("AButton")){
            Debug.Log("A pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, 1));
        }

        else if (Input.GetButtonDown("BButton")){
            Debug.Log("B pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, 1));
        }

        else if (Input.GetButtonDown("XButton")){
            Debug.Log("X pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, 1));
        }

        else if (Input.GetButtonDown("YButton")){
            Debug.Log("Y pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, 1));
        }

        if (Input.GetButtonDown("StartButton")){
            Debug.Log("Start pressed");
        }

        if (Input.GetButtonDown("SelectButton")){
            Debug.Log("Select pressed");
        }
    }

    private void CheckJoysticks()
    {
        if(Input.GetJoystickNames().Length != 0 && Input.GetJoystickNames()[0] != "") {
            if(Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus) {
                LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
                //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX,LeftJoystickXStatus));
            }

            if(Input.GetAxis("LeftJoystickY") != LeftJoystickYStatus) {
                LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
                //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
            }

            #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (Input.GetAxis("RightJoystickXMac") != RightJoystickXStatus) {
                RightJoystickXStatus = Input.GetAxis("RightJoystickXMac");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
            }
            #else
            if(Input.GetAxis("RightJoystickX") != RightJoystickXStatus) {
                RightJoystickXStatus = Input.GetAxis("RightJoystickX");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
            }
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
                //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX,LeftJoystickXStatus));
            }

            if(Input.GetAxis("KeyboardWS") != LeftJoystickYStatus) {
                LeftJoystickYStatus = Input.GetAxis("KeyboardWS");
                //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
            }

            if(Input.GetAxis("KeyboardLeftRight") != RightJoystickXStatus) {
                RightJoystickXStatus = Input.GetAxis("KeyboardLeftRight");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
            }

            if(Input.GetAxis("KeyboardUpDown") != RightJoystickYStatus) {
                RightJoystickYStatus = Input.GetAxis("KeyboardUpDown");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
            }
        }

        if(Input.GetButtonDown("LeftJoystickButton")) {
            //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickButton, 1));
            Debug.Log("Left joystick button pressed");
        }

        if(Input.GetButtonDown("RightJoystickButton")) {
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, 1));
            Debug.Log("Right joystick button pressed");
        }
    }

    private void CheckTriggers()
    {
        if (Input.GetAxis("LeftTrigger") != LeftTriggerStatus){
            LeftTriggerStatus = Input.GetAxis("LeftTrigger");
        }

        if (Input.GetAxis("RightTrigger") != RightTriggerStatus){
            RightTriggerStatus = Input.GetAxis("RightTrigger");
        }
    }

    private void CheckBumpers()
    {
        if (Input.GetButtonDown("LeftBumper")){
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftBumper, 1));
            Debug.Log("Left bumper pressed");
        }
        if (Input.GetButtonDown("RightBumper")){
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.RightBumper, 1));
            Debug.Log("Right bumper pressed");
        }
    }
}
