using UnityEngine;
using System;
using System.Collections.Generic;

public class StreamController : InputReceiver {

	[Tooltip("The speed at which the player can make the strength of a stream vary")]
	[SerializeField]
	private float strengthIncreaseSpeed = 1F;

	private static List<Stream> greenStreams;
	private static List<Stream> blueStreams;
	private static List<Stream> yellowStreams;
	private static List<Stream> redStreams;

	private EnumStreamColor selectedColor = EnumStreamColor.NONE;

	[Tooltip("The power controller to call when using a power")]
	[SerializeField]
	private PowerController powerController;

	/// <summary>
	/// Used to register a stream to the stream controller so that it can be manipulated.
	/// </summary>
	/// <param name="stream">The stream to register</param>
	/// <param name="color">The color of the stream</param>
	public static void RegisterStream(Stream stream, EnumStreamColor color) {
		GetStreamList(color).Add(stream);
	}

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		if(inputEvent.InputAxis == EnumAxis.RightTrigger) {
			if(selectedColor != EnumStreamColor.NONE) {
				GetStreamList(selectedColor).ForEach((stream) => {
					stream.IncreaseStrength(inputEvent.Value * strengthIncreaseSpeed);
				});
			}
		} else if(inputEvent.InputAxis == EnumAxis.LeftTrigger) {
			if(selectedColor != EnumStreamColor.NONE) {
				GetStreamList(selectedColor).ForEach((stream) => {
					stream.DecreaseStrength(inputEvent.Value * strengthIncreaseSpeed);
				});
			}
		} else {
			EnumButtonState state = (EnumButtonState) inputEvent.Value;
			EnumStreamColor color = EnumStreamColor.NONE;

			switch(inputEvent.InputAxis) {
				case EnumAxis.AButton:
					color = EnumStreamColor.GREEN;
					break;
				case EnumAxis.BButton:
					color = EnumStreamColor.RED;
					break;
				case EnumAxis.XButton:
					color = EnumStreamColor.BLUE;
					break;
				case EnumAxis.YButton:
					color = EnumStreamColor.YELLOW;
					break;
				default:
					break;
			}

			if(state == EnumButtonState.CLICKED) { // Player has simply pressed a button
				SwitchDirectionForColor(color);
			} else { // Player is either holding down a button or just released it after holding it down
				ChangeSelectedColor(color, state);
			}
		}
	}

	// Use this for initialization
	private void Awake() {
		if(greenStreams == null) {
			greenStreams = new List<Stream>();
		}
		if(blueStreams == null) {
			blueStreams = new List<Stream>();
		}
		if(yellowStreams == null) {
			yellowStreams = new List<Stream>();
		}
		if(redStreams == null) {
			redStreams = new List<Stream>();
		}
	}

	private void ChangeSelectedColor(EnumStreamColor color, EnumButtonState state) {
		if(color != EnumStreamColor.NONE) {
			if(state == EnumButtonState.HELD_DOWN) {
				selectedColor = color;
			} else if(selectedColor == color && state == EnumButtonState.RELEASED) {
				selectedColor = EnumStreamColor.NONE;
			}
		}
	}

	private void SwitchDirectionForColor(EnumStreamColor color) {
		(powerController.GetPower(EnumPower.SwitchDirection) as SwitchDirectionPower).streams = GetStreamList(color);
		powerController.ActivatePower(EnumPower.SwitchDirection);
	}

	private static List<Stream> GetStreamList(EnumStreamColor color) {
		switch(color) {
			case EnumStreamColor.GREEN:
				return greenStreams;
			case EnumStreamColor.BLUE:
				return blueStreams;
			case EnumStreamColor.YELLOW:
				return yellowStreams;
			case EnumStreamColor.RED:
				return redStreams;
			default:
				return null;
		}
	}
}
