using UnityEngine;
using System.Collections.Generic;

public class SwitchDirectionPower : Power {
	public List<Stream> Streams { get; set; } // The streams on which the power should apply
	//public EnumStreamColor StreamColor { get; set; }

	private void Awake() {
		Streams = new List<Stream>();
		PowerType = EnumPower.SwitchDirection;
	}

	/// <summary>
	/// Switches the direction of the streams in the current stream list. This list should always be set before calling this.
	/// </summary>
	protected override void ExecuteAction() {
		/*switch(StreamColor) {
			case EnumStreamColor.BLUE:
				cooldownColor = Color.blue;
				break;
			case EnumStreamColor.GREEN:
				cooldownColor = Color.green;
				break;
			case EnumStreamColor.RED:
				cooldownColor = Color.red;
				break;
			case EnumStreamColor.YELLOW:
				cooldownColor = Color.yellow;
				break;
			default:
				cooldownColor = Color.white;
				break;
		}*/
		Streams.ForEach((stream) => {
			stream.SwitchDirection();
		});
	}
}

