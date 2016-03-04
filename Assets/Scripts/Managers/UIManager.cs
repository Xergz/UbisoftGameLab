using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIManager : InputReceiver {

    public MainMenu mainMenuScript;
    public PauseMenu pauseMenuScript;
    public GameManager GameManager;

    void Awake() {
        mainMenuScript = gameObject.GetComponent<MainMenu>();
        pauseMenuScript = gameObject.GetComponent<PauseMenu>();
        if (GameManager.LoadCheckpointFile()) {
            mainMenuScript.transform.GetChild(0).gameObject.SetActive(true);
            mainMenuScript.eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(mainMenuScript.transform.GetChild(0).gameObject);
        }
        else {
            mainMenuScript.transform.GetChild(0).gameObject.SetActive(false);
            mainMenuScript.eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(mainMenuScript.transform.GetChild(1).gameObject);
        }
    }

    public override void ReceiveInputEvent(InputEvent inputEvent) {
        if(inputEvent.InputAxis == EnumAxis.StartButton && inputEvent.Value == 1) {
            if (pauseMenuScript != null) {
                pauseMenuScript.Pause();
            }     
        }

    }
}
