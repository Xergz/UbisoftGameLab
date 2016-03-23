using UnityEngine;
using System;
using System.Collections.Generic;

public class StreamController : InputReceiver {

	public static float OceanAreaCost { get; private set; }

	private static List<Stream> greenStreams;
	private static List<Stream> blueStreams;
	private static List<Stream> yellowStreams;
	private static List<Stream> redStreams;

	private static List<Stream>[] streamLists;

	[Tooltip("The speed at which the player can make the strength of a stream vary")]
	[SerializeField]
	private float strengthIncreaseSpeed = 1F;

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
			IncreaseStreamStrength(inputEvent);
		} else if(inputEvent.InputAxis == EnumAxis.LeftTrigger) {
			DecreaseStreamStrength(inputEvent);
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

			if(state == EnumButtonState.PRESSED) { // Player has simply pressed a button
				SwitchDirectionForColor(color);
			}
		}
	}

	/// <summary>
	/// Set the costs of all areas linked to a stream within the zone of an entity according to a vector from position to target
	/// </summary>
	/// <param name="position">The position of the entity willing to set the costs</param>
	/// <param name="target">The target position the entity wishes to reach</param>
	public static void SetAreaCosts(Vector3 position, Vector3 target) {
		foreach(List<Stream> streams in streamLists) {
			foreach(Stream stream in streams) {
				if(stream.Zone == PlayerController.CurrentZone) {
					stream.SetAreaCost(position, target);
				}
			}
		}
	}

	/// <summary>
	/// Set the costs of all areas linked to a stream within the zone of an entity to a constant cost
	/// </summary>
	/// <param name="cost">The cost to give the areas</param>
	public static void SetAreaCosts(float cost) {
		foreach(List<Stream> streams in streamLists) {
			foreach(Stream stream in streams) {
				if(stream.Zone == PlayerController.CurrentZone) {
					stream.SetAreaCost(cost);
				}
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

		streamLists = new List<Stream>[] { greenStreams, blueStreams, yellowStreams, redStreams };
		OceanAreaCost = NavMesh.GetAreaCost(NavMesh.GetAreaFromName("Ocean"));
	}

	/// <summary>
	/// Switch the direction of all streams of a color.
	/// </summary>
	/// <param name="color">The color of the streams to switch</param>
	private void SwitchDirectionForColor(EnumStreamColor color) {
		StartCoroutine(PlayerController.ActivateSwitchFX(color));
        PlayerController.SFXReverseStream();
		(powerController.GetPower(EnumPower.SwitchDirection) as SwitchDirectionPower).Streams = GetStreamList(color);
		powerController.ActivatePower(EnumPower.SwitchDirection);
	}

	private void IncreaseStreamStrength(InputEvent inputEvent) {
		if(PlayerController.isPlayerOnstream) {
			(powerController.GetPower(EnumPower.IncreaseStrength) as IncreaseStrength).value = inputEvent.Value * strengthIncreaseSpeed;
			(powerController.GetPower(EnumPower.IncreaseStrength) as IncreaseStrength).stream = PlayerController.streamPlayer;
			powerController.ActivatePower(EnumPower.IncreaseStrength);
		}
	}

	private void DecreaseStreamStrength(InputEvent inputEvent) {
		if(PlayerController.isPlayerOnstream) {
			(powerController.GetPower(EnumPower.DecreaseStrength) as DecreaseStrength).value = inputEvent.Value * strengthIncreaseSpeed;
			(powerController.GetPower(EnumPower.DecreaseStrength) as DecreaseStrength).stream = PlayerController.streamPlayer;
			powerController.ActivatePower(EnumPower.DecreaseStrength);
		}
	}


	/// <summary>
	/// Get the list of all streams of a color
	/// </summary>
	/// <param name="color">The color of the streams to get</param>
	/// <returns>The list of streams</returns>
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
