﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{

    public InputReceiver cameraController;
    public InputReceiver playerController;
    public InputReceiver streamController;
    public InputReceiver uiController;

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
    private bool pressedLeftTrigger = false;
    private bool pressedRightTrigger = false;

    void Awake()
    {
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        streamController = GameObject.Find("StreamController").GetComponent<StreamController>();
        playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
        GameObject uiManager = GameObject.Find("UIManager");
        if (uiManager == null)
        {
            uiController = null;
        }
        else {
            uiController = uiManager.GetComponent<UIManager>();
        }
    }

    void Start()
    {
        LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
        LeftJoystickYStatus = Input.GetAxis("LeftJoystickY");
        RightJoystickXStatus = Input.GetAxis("RightJoystickX");
        RightJoystickYStatus = Input.GetAxis("RightJoystickY");
        LeftTriggerStatus = Input.GetAxis("LeftTrigger");
        RightTriggerStatus = Input.GetAxis("RightTrigger");
    }


    // Update is called once per frame
    void Update()
    {
        CheckButtons();
        CheckJoysticks();
        CheckTriggers();
        CheckBumpers();
    }

    private void CheckButtons()
    {
        if (Input.GetButtonDown("AButton"))
        {
            Debug.Log("A pressed");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float)EnumButtonState.PRESSED));
            AButtonTimeAtDown = Time.time;
        }
        else if (Input.GetButtonDown("BButton"))
        {
            Debug.Log("B pressed");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float)EnumButtonState.PRESSED));
            BButtonTimeAtDown = Time.time;
        }
        else if (Input.GetButtonDown("XButton"))
        {
            Debug.Log("X pressed");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float)EnumButtonState.PRESSED));
            XButtonTimeAtDown = Time.time;
        }
        else if (Input.GetButtonDown("YButton"))
        {
            Debug.Log("Y pressed");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float)EnumButtonState.PRESSED));
            YButtonTimeAtDown = Time.time;
        }

        if (Input.GetButton("AButton"))
        {
            if (Time.time - AButtonTimeAtDown > timeBeforeHeldDown && !AButtonHeldDown)
            {
                Debug.Log("A held down");
                streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (float)EnumButtonState.HELD_DOWN));
                AButtonHeldDown = true;
            }
        }
        else if (Input.GetButton("BButton"))
        {
            if (Time.time - BButtonTimeAtDown > timeBeforeHeldDown && !BButtonHeldDown)
            {
                Debug.Log("B held down");
                streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (float)EnumButtonState.HELD_DOWN));
                BButtonHeldDown = true;
            }
        }
        else if (Input.GetButton("XButton"))
        {
            if (Time.time - XButtonTimeAtDown > timeBeforeHeldDown && !XButtonHeldDown)
            {
                Debug.Log("X held down");
                streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (float)EnumButtonState.HELD_DOWN));
                XButtonHeldDown = true;
            }
        }
        else if (Input.GetButton("YButton"))
        {
            if (Time.time - YButtonTimeAtDown > timeBeforeHeldDown && !YButtonHeldDown)
            {
                Debug.Log("Y held down");
                streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (float)EnumButtonState.HELD_DOWN));
                YButtonHeldDown = true;
            }
        }

        if (Input.GetButtonUp("AButton"))
        {
            Debug.Log("A released or clicked");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.AButton, (AButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            AButtonHeldDown = false;
        }
        else if (Input.GetButtonUp("BButton"))
        {
            Debug.Log("B released or clicked");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.BButton, (BButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            BButtonHeldDown = false;
        }
        else if (Input.GetButtonUp("XButton"))
        {
            Debug.Log("X released or clicked");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.XButton, (XButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            XButtonHeldDown = false;
        }
        else if (Input.GetButtonUp("YButton"))
        {
            Debug.Log("Y released or clicked");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.YButton, (YButtonHeldDown) ?
                                                                                (float)EnumButtonState.RELEASED :
                                                                                (float)EnumButtonState.CLICKED));
            YButtonHeldDown = false;
        }

        if (Input.GetButtonDown("StartButton"))
        {
            Debug.Log("Start pressed");
            if (uiController != null)
                uiController.ReceiveInputEvent(new InputEvent(EnumAxis.StartButton, 1));
        }

        if (Input.GetButtonDown("SelectButton"))
        {
            Debug.Log("Select pressed");
        }
    }

    private void CheckJoysticks()
    {
        if (Input.GetJoystickNames().Length != 0 && Input.GetJoystickNames()[0] != "")
        {
            if (Input.GetAxis("LeftJoystickX") != LeftJoystickXStatus)
            {
                LeftJoystickXStatus = Input.GetAxis("LeftJoystickX");
                playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, LeftJoystickXStatus));
            }

            if (Input.GetAxis("LeftJoystickY") != LeftJoystickYStatus)
            {
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
            if (Input.GetAxis("RightJoystickY") != RightJoystickYStatus)
            {
                RightJoystickYStatus = Input.GetAxis("RightJoystickY");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
            }
#endif
        }
        else {
            if (Input.GetAxis("KeyboardAD") != LeftJoystickXStatus)
            {
                LeftJoystickXStatus = Input.GetAxis("KeyboardAD");
                playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickX, LeftJoystickXStatus));
            }

            if (Input.GetAxis("KeyboardWS") != LeftJoystickYStatus)
            {
                LeftJoystickYStatus = Input.GetAxis("KeyboardWS");
                playerController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftJoystickY, LeftJoystickYStatus));
            }

            RightJoystickXStatus = Input.GetAxis("KeyboardLeftRight");
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickX, RightJoystickXStatus));

            if (Input.GetAxis("KeyboardUpDown") != RightJoystickYStatus)
            {
                RightJoystickYStatus = Input.GetAxis("KeyboardUpDown");
                cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickY, RightJoystickYStatus));
            }
        }

        if (Input.GetButtonDown("LeftJoystickButton"))
        {
            Debug.Log("Left joystick button pressed");
        }

        if (Input.GetButtonDown("RightJoystickButton"))
        {
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, (float)EnumButtonState.PRESSED));
            Debug.Log("Right joystick button pressed");
        }

        if (Input.GetButtonUp("RightJoystickButton"))
        {
            cameraController.ReceiveInputEvent(new InputEvent(EnumAxis.RightJoystickButton, (float)EnumButtonState.RELEASED));
            Debug.Log("Right joystick button released");
        }
    }

    private void CheckTriggers()
    {

        if (Input.GetAxis("LeftTrigger") > 0.01f)
        {
            //Debug.Log("Left triigger pressed");
            LeftTriggerStatus = Input.GetAxis("LeftTrigger");
            pressedLeftTrigger = true;
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftTrigger, LeftTriggerStatus));
        }
        else if (Input.GetAxis("LeftTrigger") == 0 && pressedLeftTrigger)
        {
            //Debug.Log("Left trigger reset");
            pressedLeftTrigger = false;
            LeftTriggerStatus = Input.GetAxis("LeftTrigger");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftTrigger, LeftTriggerStatus));

        }

        if (Input.GetAxis("RightTrigger") > 0.01f)
        {
            //Debug.Log("Right trigger pressed");
            RightTriggerStatus = Input.GetAxis("RightTrigger");
            pressedRightTrigger = true;
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.RightTrigger, RightTriggerStatus));
        }
        else if (Input.GetAxis("RightTrigger") == 0 && pressedRightTrigger)
        {
            //Debug.Log("right trigger reset");
            pressedRightTrigger = false;
            RightTriggerStatus = Input.GetAxis("RightTrigger");
            streamController.ReceiveInputEvent(new InputEvent(EnumAxis.RightTrigger, RightTriggerStatus));

        }


    }

    private void CheckBumpers()
    {
        if (Input.GetButtonDown("LeftBumper"))
        {
            Debug.Log("Left bumper pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.LeftTrigger, 1));
        }
        if (Input.GetButtonDown("RightBumper"))
        {
            Debug.Log("Right bumper pressed");
            //streamController.ReceiveInputEvent(new InputEvent(EnumAxis.RightTrigger, 1));
        }
    }
}
