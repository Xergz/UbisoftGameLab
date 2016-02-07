public class InputEvent {
	public EnumAxis inputAxis { get; set; }
	public float value { get; set; }

	public InputEvent(EnumAxis axis, float value) {
		inputAxis = axis;
		this.value = value;
	}
}
