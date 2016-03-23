using UnityEngine;
using System.Collections;

public class UIButtonHighlight : InputReceiver {

    public GameObject highlightA;
    public GameObject highlightB;
    public GameObject highlightX;
    public GameObject highlightY;

    // Use this for initialization
    void Start () {
        highlightA.SetActive(false);
        highlightB.SetActive(false);
        highlightX.SetActive(false);
        highlightY.SetActive(false);
    }

    public override void ReceiveInputEvent(InputEvent inputEvent) {
        EnumButtonState state = (EnumButtonState)inputEvent.Value;
        switch (inputEvent.InputAxis) {
            case EnumAxis.AButton:
                if (state == EnumButtonState.PRESSED)
                    highlightA.SetActive(true);
                else if (state == EnumButtonState.RELEASED || state == EnumButtonState.CLICKED)
                    highlightA.SetActive(false);
                break;
            case EnumAxis.BButton:
                if (state == EnumButtonState.PRESSED)
                    highlightB.SetActive(true);
                else if (state == EnumButtonState.RELEASED || state == EnumButtonState.CLICKED)
                    highlightB.SetActive(false);
                break;
            case EnumAxis.XButton:
                if (state == EnumButtonState.PRESSED)
                    highlightX.SetActive(true);
                else if (state == EnumButtonState.RELEASED || state == EnumButtonState.CLICKED)
                    highlightX.SetActive(false);
                break;
            case EnumAxis.YButton:
                if (state == EnumButtonState.PRESSED)
                    highlightY.SetActive(true);
                else if (state == EnumButtonState.RELEASED || state == EnumButtonState.CLICKED)
                    highlightY.SetActive(false);
                break;
            default:
                break;
        }
    }
}
