using UnityEngine;
using System;
using System.Collections.Generic;

public class StreamController : InputReceiver {

	private static List<Stream> greenStreams;
	private static List<Stream> blueStreams;
	private static List<Stream> yellowStreams;
	private static List<Stream> redStreams;

	private EnumStreamColor selectedColor = EnumStreamColor.NONE;

	/// <summary>
	/// Used to register a stream to the stream controller so that it can be manipulated.
	/// </summary>
	/// <param name="stream">The stream to register</param>
	/// <param name="color">The color of the stream</param>
	public static void Register(Stream stream, EnumStreamColor color) {
		GetStreamList(color).Add(stream);
	}

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		switch(inputEvent.inputAxis) {
			case EnumAxis.AButton:
				ChangeSelectedColor(EnumStreamColor.GREEN, inputEvent.value);
				break;
			case EnumAxis.BButton:
				ChangeSelectedColor(EnumStreamColor.RED, inputEvent.value);
				break;
			case EnumAxis.XButton:
				ChangeSelectedColor(EnumStreamColor.BLUE, inputEvent.value);
				break;
			case EnumAxis.YButton:
				ChangeSelectedColor(EnumStreamColor.YELLOW, inputEvent.value);
				break;
			case EnumAxis.RightBumper:
				if(selectedColor != EnumStreamColor.NONE) {
					GetStreamList(selectedColor).ForEach((stream) => {
						stream.SwitchDirection();
					});
				}
				break;
			default:
				break;
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

	private void ChangeSelectedColor(EnumStreamColor color, float value) {
		if(value > 0) {
			selectedColor = color;
		} else if(selectedColor == color) {
			selectedColor = EnumStreamColor.NONE;
		}
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
