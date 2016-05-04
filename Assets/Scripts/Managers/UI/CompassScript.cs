﻿using UnityEngine;
using System.Collections.Generic;

public class CompassScript : MonoBehaviour {

	public GameObject imageDirectionCenter;

	public GameObject compassDirection;
	public Vector3 imageCenter;
	public Vector3 playerPosition;
	public Vector3 nextFragmentPosition;

	public Vector3 playerForwardVector;
	public Vector3 playerFragmentVector;

	[Tooltip("List of the places to point to, must be ordered the same way as the list of fragments")]
	public List<LevelWaypoint> targets;

	public float lastAngle;
	public float currentAngle;

	void Start() {
		imageCenter = imageDirectionCenter.transform.position;
		playerPosition = PlayerController.playerRigidbody.position;
		//nextFragmentPosition = PlayerController.fragmentsList[PlayerController.nextFragmentIndex].position;

		//CalculateVector();

		lastAngle = 0.0F;
	}

	void Update() {
		if(targets[PlayerController.nextFragmentFind].playerEntered) {
			nextFragmentPosition = PlayerController.fragmentsList[PlayerController.nextFragmentFind].position;
		} else {
			nextFragmentPosition = targets[PlayerController.nextFragmentFind].transform.position;
		}
		if(PlayerController.nextFragmentFind < PlayerController.numberOfFragments) {
			CalculateVector();
			RotateDirection(CalculateAngle() - lastAngle);
			lastAngle = currentAngle;

			playerPosition = PlayerController.playerRigidbody.position;
		} else {
			RotateDirection(-lastAngle);
			lastAngle = 0.0F;
		}
	}

	void RotateDirection(float angle) {
		compassDirection.GetComponent<RectTransform>().RotateAround(imageCenter, Vector3.forward, angle);
	}

	public void CalculateVector() {
		playerForwardVector = -Vector3.forward;
		playerForwardVector.y = 0;
		playerFragmentVector = new Vector3(nextFragmentPosition.x - playerPosition.x, 0, nextFragmentPosition.z - playerPosition.z);
	}

	public float CalculateAngle() {
		currentAngle = Vector3.Angle(playerForwardVector, playerFragmentVector) - 10.0F;
		if(Vector3.Cross(playerForwardVector, playerFragmentVector).y > 0) {
			currentAngle = -currentAngle;
		}
		return currentAngle;
	}
}