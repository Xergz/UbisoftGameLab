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
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, 1));
        }

        if (Input.GetButtonDown("BButton")){
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, 1));
        }

        if (Input.GetButtonDown("XButton")){
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, 1));
        }

        if (Input.GetButtonDown("YButton")){
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, 1));
        }

        if (Input.GetButtonDown("StartButton")){

        }

        if (Input.GetButtonDown("SelectButton")){
            
        }
    }

    private void CheckJoysticks()
    {
        if (Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus){
            LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
            //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX,LeftJoystickXStatus));
        }

        if (Input.GetAxis("LeftJoystickY") != LeftJoystickYStatus){
            LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
            //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
        }

        if (Input.GetButtonDown("LeftJoystickButton")){
            //playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickButton, 1));
        }

        if (Input.GetAxis("RightJoystickX") != RightJoystickXStatus){
            RightJoystickXStatus = Input.GetAxis("RightJoystickX");
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));
        }

        if (Input.GetAxis("RightJoystickY") != RightJoystickYStatus){
            RightJoystickYStatus = Input.GetAxis("RightJoystickY");
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
        }

        if (Input.GetButtonDown("RightJoystickButton")){
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, 1));
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

        }
        if (Input.GetButtonDown("RightBumper")){

        }
    }
}
