using UnityEngine;

public abstract class InputReceiver : MonoBehaviour {
    public abstract void ReceiveInputEvent(InputEvent inputEvent);
}
