using UnityEngine;
using System.Collections.Generic;

public class PlayerController : InputReceiver {

	public Rigidbody playerRigidbody;

	[Tooltip("The force to apply to the player when it moves (multiplied by its movement speed multiplier)")]
	public float movementForce;
	[Tooltip("The time in seconds the player takes to rotate 1 unit")]
	public float rotationSpeed;
	[Tooltip("The maximum velocity the player can reach")]
	public float maximumVelocity;


	private static List<Fragment> memoryFragments; // The list of all the fragments in the player's possession. Also the number of life he has.

	private float ZSpeedMultiplier = 0; // The current Z speed multiplier
	private float XSpeedMultiplier = 0; // The current X speed multiplier

	private float currentVelocity; // The current velocity of the player

	private Vector3 forceToApply;


	public override void ReceiveInputEvent(InputEvent inputEvent) {
		if(inputEvent.InputAxis == EnumAxis.LeftJoystickX) {
			XSpeedMultiplier = inputEvent.Value;
            if (Mathf.Abs(XSpeedMultiplier) < 0.2) {
                XSpeedMultiplier = 0;
            }
        }

		if(inputEvent.InputAxis == EnumAxis.LeftJoystickY) {
			ZSpeedMultiplier = inputEvent.Value;
            if (Mathf.Abs(ZSpeedMultiplier) < 0.2) {
                ZSpeedMultiplier = 0;
            }
        }
	}

	public void AddForce(Vector3 force) {
		forceToApply += force;
	}

	public void AddFragment(Fragment fragment) {
		memoryFragments.Add(fragment);
		Debug.Log("Plus one life! Congratulations! You gained the \"" + fragment.fragmentName + "\" memory fragment");
	}

	public void DamagePlayer(int fragmentNb) {
		for(int i = 0; i < fragmentNb && memoryFragments.Count > 0; ++i) {
			int index = Random.Range(0, memoryFragments.Count - 1);
			Fragment lostFragment = memoryFragments[index];
			memoryFragments.RemoveAt(index);
			Debug.Log("Ouch! You took damage... You lost the \"" + lostFragment.fragmentName + "\" memory fragment");
		}
	}

	public List<Fragment> GetFragments() {
		return memoryFragments;
	}


	private void Awake() {
		memoryFragments = new List<Fragment>();
		forceToApply = new Vector3(0, 0, 0);

		if(playerRigidbody == null) {
			Debug.LogError("No player is registered to the PlayerController");
		} else {
			playerRigidbody.GetComponent<Player>().PlayerController = this;
		}
	}

	private void FixedUpdate() {
		MovePlayer();
	}



	private void MovePlayer() {
		Vector3 movement = new Vector3(movementForce * XSpeedMultiplier, 0, movementForce * ZSpeedMultiplier) + forceToApply;

		if(!(Mathf.Approximately(movement.x, 0F) && Mathf.Approximately(movement.y, 0F) && Mathf.Approximately(movement.z, 0F))) {
			playerRigidbody.AddForce(movement, ForceMode.Acceleration);


            Vector3 lastForward = playerRigidbody.transform.forward;
			lastForward.y = 0;

			// Check in what direction the boat should rotate
            float rotation = Vector3.Angle(lastForward, movement);
			if(Vector3.Dot(Vector3.up, Vector3.Cross(lastForward, movement)) < 0) {
				rotation = -rotation;
			}

            rotation = Mathf.SmoothDampAngle(0, rotation, ref currentVelocity, rotationSpeed);
            playerRigidbody.transform.Rotate(0, rotation, 0, Space.World);
        }

		if(Vector3.Magnitude(playerRigidbody.velocity) > maximumVelocity) {
			playerRigidbody.velocity = Vector3.Normalize(playerRigidbody.velocity) * maximumVelocity;
		}

		forceToApply = new Vector3(0, 0, 0);
	}
}
