using UnityEngine;

public abstract class InputReceiver : MonoBehaviour {
	/// <summary>
	/// Receive an input from the controller or the keyboard
	/// </summary>
	/// <param name="inputEvent">An object containing the input received and its value</param>
	public abstract void ReceiveInputEvent(InputEvent inputEvent);
}
