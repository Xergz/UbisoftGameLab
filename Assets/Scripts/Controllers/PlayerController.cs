﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : InputReceiver {
	public static Rigidbody playerRigidbody;

	public static MusicController Music;

	private static EnumZone c_currentZone;
	public static EnumZone CurrentZone {
		get {
			return c_currentZone;
		}
		set {
			c_currentZone = value;

			if(Music != null) {
				Music.OnZoneChanged(c_currentZone);
			}
		}
	}

	private bool canBeMoved = true;
	public bool PlayerCanBeMoved {
		get {
			return canBeMoved;
		}
		set {
			canBeMoved = value;
			if(!canBeMoved) {
				forceToApply = Vector3.zero;
			}
		}
	}

	public static bool IsDead { get; private set; }
	public static bool HasWon { get; private set; }
	public static bool isPlayerOnstream { get; set; }

	public static Stream streamPlayer { get; set; }

	[Tooltip("The force to apply to the player when it moves (multiplied by its movement speed multiplier)")]
	public float movementForce;
	[Tooltip("The time in seconds the player takes to rotate 1 unit")]
	public float rotationSpeed;
	[Tooltip("The maximum velocity the player can reach")]
	public float maximumVelocity;
	[Tooltip("The range the player's sight can reach. We should animate any objet within this distance")]
	public float sightRange = 60F;

	public Image lifeBarFill;
	public Image lifeBarRim;

	private static Image lifeBarFillStatic;
	private static Image lifeBarRimStatic;

	//public PowerController powerController;

	public static int baseLife = 100;

	private static float maxFill;
	private static int maxLife;

	public static List<Fragment> memoryFragments; // The list of all the fragments in the player's possession.

	//public Transform Fragments;
	public static List<Transform> fragmentsList; //List of every fragments

	//public static Transform nextFragment;
	public static int nextFragmentIndex;
	public static int numberOfFragments;

	private float ZSpeedMultiplier = 0; // The current Z speed multiplier
	private float XSpeedMultiplier = 0; // The current X speed multiplier

	private float currentVelocity; // The current velocity of the player

	private Vector3 forceToApply;



	[Tooltip("The current life of the player")]
	[SerializeField]
	private int life;

	private static int currentLife;

	public static GameObject Player {
		get {
			if(playerRigidbody != null) {
				return playerRigidbody.gameObject;
			}
			return null;
		}
	}

	public static List<Fragment> GetCollectedFragments() {
		return memoryFragments;
	}

	public static int GetPlayerCurrentLife() {
		return currentLife;
	}

	public static void SetPlayerCurrentLife(int val) {
		int maxLife = baseLife * (int) ((memoryFragments.Count + 1) * 0.2F);

		currentLife = Mathf.Clamp(val, 0, maxLife);
	}

	public void AddLife(int val) {
		currentLife = Mathf.Clamp(currentLife + val, 0, maxLife);
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

	public void AddForce(Vector3 force, Stream stream) {
		if(PlayerCanBeMoved) {
			forceToApply += force;
			if(force == Vector3.zero) {
				isPlayerOnstream = false;
				streamPlayer = null;
			} else {
				isPlayerOnstream = true;
				streamPlayer = stream;
			}
		}
	}

	public static void AddFragment(Fragment fragment) {
		memoryFragments.Add(fragment);

		RestoreAllLife();

		//powerController.SetCooldownMultipliers(maxFill);

		nextFragmentIndex++;
	}

	public void ClearFragments() {
		memoryFragments.Clear();
		nextFragmentIndex = 0;
	}

	public static void RegisterFragment(Fragment fragment) {
		//fragmentsList.Insert(fragment.index, fragment.GetComponent<Transform>());
		UpdateNumberOfFragments();
	}

	public static void UpdateNumberOfFragments() {
		numberOfFragments = fragmentsList.Count;
	}

	public void DamagePlayer(int damage) {
		currentLife -= damage;
		float percentFilled = ((float) currentLife / (float) maxLife);
		if(currentLife <= 5) {
			lifeBarFillStatic.color = Color.red;
		} else {
			lifeBarFillStatic.color = (percentFilled >= 0.5F) ? Color.Lerp(Color.yellow, Color.green, (percentFilled - 0.5F) * 2) : Color.Lerp(Color.red, Color.yellow, percentFilled * 2);
		}
		lifeBarFillStatic.fillAmount = percentFilled * maxFill;
	}

	public List<Fragment> GetFragments() {
		return memoryFragments;
	}


	private void Awake() {
		playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();

		GameObject musicControllerObject = GameObject.Find("MusicController");
		if(musicControllerObject != null) {
			Music = musicControllerObject.GetComponent<MusicController>();
		}

		memoryFragments = new List<Fragment>();
		forceToApply = new Vector3(0, 0, 0);
		CurrentZone = EnumZone.OPEN_WORLD;
		PlayerCanBeMoved = true;

		fragmentsList = new List<Transform>();
		numberOfFragments = fragmentsList.Count;
		nextFragmentIndex = 0;

		lifeBarFillStatic = lifeBarFill;
		lifeBarRimStatic = lifeBarRim;

		RestoreAllLife();

		if(playerRigidbody == null) {
			Debug.LogError("No player is registered to the PlayerController");
		} else {
			playerRigidbody.GetComponent<Player>().PlayerController = this;
		}
	}

	private void FixedUpdate() {
		life = currentLife;
		if(PlayerCanBeMoved) {
			MovePlayer();
		} else {
			playerRigidbody.velocity = Vector3.zero;
		}
	}

	private void MovePlayer() {
		//var cam = Camera.main;

		Vector3 baseMovement = new Vector3(movementForce * XSpeedMultiplier, 0, movementForce * ZSpeedMultiplier);
		Vector3 movement = /*Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) **/ baseMovement + forceToApply; //Adjust the movement direction depending on camera before applying external forces

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

	private static void RestoreAllLife() {
		maxFill = (memoryFragments.Count + 1) * 0.2F;
		maxLife = (int) (baseLife * maxFill);
		currentLife = maxLife;

		lifeBarRimStatic.fillAmount = maxFill;
		lifeBarFillStatic.fillAmount = maxFill;
		lifeBarFillStatic.color = Color.green;
	}
}
