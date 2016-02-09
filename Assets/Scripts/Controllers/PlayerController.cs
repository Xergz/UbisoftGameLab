using UnityEngine;
using System.Collections.Generic;

public class PlayerController : InputReceiver {
	public Rigidbody playerRigidbody;

	[Tooltip("The force to apply to the player when it moves (multiplied by its movement speed multiplier)")]
	public float movementForce;
	[Tooltip("The time in seconds the player takes to rotate 1 unit")]
	public float rotationSpeed;


	private float ZSpeedMultiplier = 0; // The current Z speed multiplier
	private float XSpeedMultiplier = 0; // The current X speed multiplier

    private float velo;
	private Vector3 currentVelocity; // The current velocity of the player

	private static List<Fragment> memoryFragments; // The list of all the fragments in the player's possession. Also the number of life he has.


	public override void ReceiveInputEvent(InputEvent inputEvent) {
		if(inputEvent.InputAxis == EnumAxis.LeftJoystickX) {
			XSpeedMultiplier = inputEvent.Value;
            if (Mathf.Abs(XSpeedMultiplier) < 0.2) {
                XSpeedMultiplier = 0;
            }
        }

		if(inputEvent.InputAxis == EnumAxis.LeftJoystickY) {
			ZSpeedMultiplier = -inputEvent.Value;
            if (Mathf.Abs(ZSpeedMultiplier) < 0.2) {
                ZSpeedMultiplier = 0;
            }
        }
	}

	public static void AddFragment(Fragment fragment) {
		memoryFragments.Add(fragment);
	}

	public List<Fragment> GetFragments() {
		return memoryFragments;
	}


	private void Awake() {
		memoryFragments = new List<Fragment>();

		if(playerRigidbody == null) {
			Debug.LogError("No player is registered to the PlayerController");
		}
	}

	private void FixedUpdate() {
		MovePlayer();
	}



	private void MovePlayer() {
		Vector3 movement = new Vector3(movementForce * XSpeedMultiplier, 0, movementForce * ZSpeedMultiplier);

		if(!(Mathf.Approximately(movement.x, 0F) && Mathf.Approximately(movement.y, 0F) && Mathf.Approximately(movement.z, 0F))) {
			playerRigidbody.AddForce(movement, ForceMode.Acceleration);

			currentVelocity = playerRigidbody.velocity;


            Vector3 temp = playerRigidbody.transform.forward;
            Vector3 temp2 = movement;
            temp.y = 0;
            temp2.y = 0;

            float rotation = Vector3.Angle(temp, temp2);
            rotation = Mathf.SmoothDampAngle(0, rotation, ref velo, rotationSpeed);
            playerRigidbody.transform.Rotate(0, rotation, 0, Space.World);

			//Debug.Log("LookAt: X(" + lookAt.x + "), Y(" + lookAt.y + "), Z(" + lookAt.z + ")");
        }
	}
}
