using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    [SerializeField]
    private IInputReceiver cameraController;
    [SerializeField]
    private IInputReceiver playerController;
    [SerializeField]
    private IInputReceiver streamController;

    private float LeftJoystickXStatus;
    private float LeftJoystickYStatus;
    private float RightJoystickXStatus;
    private float RightJoystickYStatus;
    private float LeftTriggerStatus;
    private float RightTriggerStatus;
    private float LeftBumperStatus;
    private float RightBumperStatus;

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
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, 1));
        }

        if (Input.GetButtonDown("BButton")){
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, 1));
        }

        if (Input.GetButtonDown("XButton")){
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, 1));
        }

        if (Input.GetButtonDown("YButton")){
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, 1));
        }

        if (Input.GetButtonDown("StartButton")){

        }

        if (Input.GetButtonDown("SelectButton")){
            
        }
    }

    private void CheckJoysticks()
    {
        if (Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus){
            playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, Input.GetAxis("LeftJoystickX")));
        }

        if (Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus){
            playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, Input.GetAxis("LeftJoystickX")));
        }

        if (Input.GetButtonDown("LeftJoystickButton")){
            playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickButton, 1));
        }

        if (Input.GetAxis("RightJoystickX") != RightJoystickXStatus){
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, Input.GetAxis("RightJoystickX")));
        }

        if (Input.GetAxis("RightJoystickY") != RightJoystickYStatus){
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, Input.GetAxis("RightJoystickY")));
        }

        if (Input.GetButtonDown("RightJoystickButton")){
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, 1));
        }
    }

    private void CheckTriggers()
    {
        if (Input.GetAxis("LeftTrigger") != LeftTriggerStatus){

        }

        if (Input.GetAxis("RightTrigger") != RightTriggerStatus){

        }
    }

    private void CheckBumpers()
    {
        if (Input.GetButtonDown("LeftBumper")){

        }
        if (Input.GetButtonDown("RightBumper")){

        }
    }
}
