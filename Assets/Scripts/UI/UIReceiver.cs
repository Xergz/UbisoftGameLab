using UnityEngine;
using System.Collections;

public class UIReceiver : InputReceiver {

    public MainMenu mainMenuScript;
    public PauseMenu pauseMenuScript;

    void Start() {
        mainMenuScript = gameObject.GetComponent<MainMenu>();
        pauseMenuScript = gameObject.GetComponent<PauseMenu>();
    }

    public override void ReceiveInputEvent(InputEvent inputEvent) {
        if(inputEvent.InputAxis == EnumAxis.SelectButton && inputEvent.Value == 1) {
            if (pauseMenuScript != null) {
                pauseMenuScript.Pause();
            }     
        }

    }
}
