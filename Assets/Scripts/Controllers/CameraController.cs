using UnityEngine;
using System.Collections;

public class CameraController : InputReceiver {
	public Transform playerTransform;

	public float distance = 5f;
	public float distanceDefault = 5f;
	public float distanceMin = 3f;
	public float distanceMax = 10f;
	public float autoAdjustTime = 0.5f;
	[Tooltip("The minimum speed of the camera in unit/second")]
	public float minSpeed = 3F;
	[Tooltip("The minimum speed the player must have to be considered moving")]
	public float minPlayerVelocity = 0.05F;

	public Vector3 movementSmooth = new Vector3(0.05f, 0.1f, 0.05f);
	public Vector2 rotation = new Vector2(0f, 40f);
	public Vector2 controllerSensitivity = new Vector2(2f, 2f);

	private Vector2 controllerInput = new Vector2(0f, 0f);
	private Vector3 positionTarget = Vector3.zero;
	[SerializeField]
	private Vector3 oldPlayerPosition = Vector3.zero;
	private Vector3 offset;
	//private float lastTimeAdjusted = 0.0f;

	[SerializeField]
	private float timeAtPlayerMoved = 0F;
	[SerializeField]
	private bool playerMoving = false;
	[SerializeField]
	private bool cameraMoving = false;
	[SerializeField]
	private float speed;

	public override void ReceiveInputEvent(InputEvent inputEvent) {
		/*
		if(inputEvent.InputAxis == EnumAxis.RightJoystickX) {
			controllerInput.x = inputEvent.Value;
			if(Mathf.Abs(controllerInput.x) < 0.2) {
				controllerInput.x = 0;
			} else {
				lastTimeAdjusted = 0.0f;
			}
			
		}

		if(inputEvent.InputAxis == EnumAxis.RightJoystickY) {
			controllerInput.y = inputEvent.Value * -1;
			if(Mathf.Abs(controllerInput.y) < 0.2) {
				controllerInput.y = 0;
			} else {
				lastTimeAdjusted = 0.0f;
			}
		}

		if(inputEvent.InputAxis == EnumAxis.RightJoystickButton) {
			if((EnumButtonState) inputEvent.Value == EnumButtonState.PRESSED) {
				Reset();
			}
		}*/
	}

	public void SetCameraPosition(Vector3 playerPosition) {
		oldPlayerPosition = playerPosition;

		transform.position = oldPlayerPosition + offset;
	}

	void Start() {
		if(playerTransform == null) {
			Debug.LogError("No transform is linked to the camera controller");
		}

		oldPlayerPosition = playerTransform.position;

		offset = Quaternion.Euler(rotation.y, rotation.x, 0) * new Vector3(0, 0, -distanceDefault);

		transform.position = oldPlayerPosition + offset;
		transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);

		//distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		//distanceDefault = Mathf.Clamp(distanceDefault, distanceMin, distanceMax);
	}

	void LateUpdate() {
		//lastTimeAdjusted += Time.deltaTime;

		if(playerTransform == null)
			return;

		Vector3 velocity = playerTransform.GetComponent<Rigidbody>().velocity;
		speed = new Vector2(velocity.x, velocity.z).magnitude;
		if(speed > minPlayerVelocity) {
			if(!playerMoving) {
				if(!cameraMoving) {
					timeAtPlayerMoved = Time.time;
				}
				playerMoving = true;
			}
			oldPlayerPosition = playerTransform.position;
		} else {
			playerMoving = false;
		}
		speed = Mathf.Max(minSpeed, speed);

		//CalculateMovement();
		CalculateCameraTarget();
		UpdatePosition();
	}

	void CalculateMovement() {
		/*distance -= controllerInput.y * controllerSensitivity.y;
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);*/

		/*rotation.x += controllerInput.x * controllerSensitivity.x;
		if(rotation.x > 360) {
			rotation.x -= 360;
		} else if(rotation.x < 360) {
			rotation.x += 360;
		}*/
	}

	void CalculateCameraTarget() {
		/*Vector3 direction = new Vector3(0, 0, -distance);

		Quaternion rotate;
		if(lastTimeAdjusted >= autoAdjustTime) {
			//Make a smooth transition towards where the player is pointing
			float currentAngle = transform.eulerAngles.y;
			rotation.x = Mathf.LerpAngle(currentAngle, playerTransform.eulerAngles.y, Time.deltaTime);
			rotate = Quaternion.Euler(rotation.y, rotation.x, 0);
		} else {
			rotate = Quaternion.Euler(rotation.y, rotation.x, 0);
		}

		direction = rotate * direction;*/
		if((playerMoving || Vector3.Distance(oldPlayerPosition + offset, transform.position) > 0.02) && Time.time - timeAtPlayerMoved > autoAdjustTime) {
			positionTarget = Vector3.MoveTowards(transform.position, oldPlayerPosition + offset, speed * Time.deltaTime);
			cameraMoving = true;
		} else {
			positionTarget = transform.position;
			cameraMoving = false;
		}

		
	}

	void UpdatePosition() {
		transform.position = positionTarget;
		//transform.LookAt(playerTransform);
	}

	void Reset() {
		distance = distanceDefault;
		rotation = new Vector2(0f, 40f);
	}
}
