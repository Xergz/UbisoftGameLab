using UnityEngine;
using System.Collections.Generic;

public class PlayerController : InputReceiver {

	public static EnumZone CurrentZone { get; set; }
	public bool PlayerCanBeMoved { get; set; }

	[Tooltip("The player's rigidbody")]
	public Rigidbody playerRigidbody;

	[Tooltip("The force to apply to the player when it moves (multiplied by its movement speed multiplier)")]
	public float movementForce;
	[Tooltip("The time in seconds the player takes to rotate 1 unit")]
	public float rotationSpeed;
	[Tooltip("The maximum velocity the player can reach")]
	public float maximumVelocity;
	[Tooltip("The range the player's sight can reach. We should animate any objet within this distance")]
 	public float sightRange = 60F;

	private List<Fragment> memoryFragments; // The list of all the fragments in the player's possession.

	private float ZSpeedMultiplier = 0; // The current Z speed multiplier
	private float XSpeedMultiplier = 0; // The current X speed multiplier

	private float currentVelocity; // The current velocity of the player

	private Vector3 forceToApply;

    private int currentLife;

	public GameObject Player {
		get {
			return playerRigidbody.gameObject;
		}
	}

    public int GetPlayerMaxLife() {
        return 10;
    }

	// TODO: Complete this method
	public int GetPlayerCurrentLife() {
        return currentLife;
	}
        
	public void SetPlayerCurrentLife(int val) {
        int maxLife = GetPlayerMaxLife();

        // Life cannot be negative
        if (val < 0) {
            currentLife = 0;
        } 
        // Life cannot be superior to the max value
        else if (val > maxLife) {
            currentLife = maxLife;
        } 
        else {
            currentLife = val;
        }
	}

    public void AddLife(int val) {
        SetPlayerCurrentLife (currentLife + val);
    }

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		if(inputEvent.InputAxis == EnumAxis.LeftJoystickX) {
			XSpeedMultiplier = inputEvent.Value;
			if(Mathf.Abs(XSpeedMultiplier) < 0.2) {
				XSpeedMultiplier = 0;
			}
		}

		if(inputEvent.InputAxis == EnumAxis.LeftJoystickY) {
			ZSpeedMultiplier = inputEvent.Value;
			if(Mathf.Abs(ZSpeedMultiplier) < 0.2) {
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
		CurrentZone = EnumZone.OPEN_WORLD;
		PlayerCanBeMoved = true;

		if(playerRigidbody == null) {
			Debug.LogError("No player is registered to the PlayerController");
		} else {
			playerRigidbody.GetComponent<Player>().PlayerController = this;
		}
	}

	private void FixedUpdate() {
		if (PlayerCanBeMoved) {
			MovePlayer();
		}
	}

	private void MovePlayer()
	{
		var cam = Camera.main;

		Vector3 baseMovement = new Vector3(movementForce * XSpeedMultiplier, 0, movementForce * ZSpeedMultiplier);
		Vector3 movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * baseMovement + forceToApply; //Adjust the movement direction depending on camera before applying external forces

		if (!(Mathf.Approximately(movement.x, 0F) && Mathf.Approximately(movement.y, 0F) && Mathf.Approximately(movement.z, 0F)))
		{
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

		if (Vector3.Magnitude(playerRigidbody.velocity) > maximumVelocity)
		{
			playerRigidbody.velocity = Vector3.Normalize(playerRigidbody.velocity) * maximumVelocity;
		}

		forceToApply = new Vector3(0, 0, 0);
	}
}
