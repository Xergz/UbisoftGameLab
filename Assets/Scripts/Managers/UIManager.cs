
public class UIManager : InputReceiver {

	public static UIManager instance = null;

	public MainMenu mainMenuScript;
	public PauseMenu pauseMenuScript;
    public EnterLevel enterLevelScript;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		GameManager.LoadCheckpointFile(false);
	}

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		if(inputEvent.InputAxis == EnumAxis.StartButton && inputEvent.Value == 1) {
			pauseMenuScript.Pause();
		}
	}

    public void EnterLevel(string levelName) {
        enterLevelScript.DisplayLevel(levelName);
    }

#if UNITY_EDITOR
	public void CallOnLevelWasLoaded(int level) {
		mainMenuScript.OnLevelWasLoaded(level);
		pauseMenuScript.OnLevelWasLoaded(level);
	}
#endif
}
