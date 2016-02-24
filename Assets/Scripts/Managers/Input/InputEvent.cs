public class InputEvent {
	public EnumAxis InputAxis { get; set; }
	public float Value { get; set; }

	public InputEvent(EnumAxis axis, float value) {
		InputAxis = axis;
		Value = value;
	}
}
