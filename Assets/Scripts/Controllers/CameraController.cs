using UnityEngine;
using System.Collections;

public class CameraController : InputReceiver {
    public Transform playerTransform;

    public float distance = 5f;
    public float distanceDefault = 5f;
    public float distanceMin = 3f;
    public float distanceMax = 10f;
    public Vector3 movementSmooth = new Vector3(0.05f, 0.1f, 0.05f);
    public Vector2 rotation = new Vector2(0f, 40f);
    public Vector2 controllerSensitivity = new Vector2(2f, 2f);

    private Vector2 controllerInput = new Vector2(0f, 0f);
    private Vector3 positionTarget = Vector3.zero;


	public override void ReceiveInputEvent(InputEvent inputEvent) {
        if(inputEvent.inputAxis == EnumAxis.RightJoystickX) {
            controllerInput.x = inputEvent.value;
            if(Mathf.Abs(controllerInput.x) < 0.2) {
                controllerInput.x = 0;
            }
        }

        if(inputEvent.inputAxis == EnumAxis.RightJoystickY) {
            controllerInput.y = inputEvent.value*-1;
            if(Mathf.Abs(controllerInput.y) < 0.2) {
                controllerInput.y = 0;
            }
        }

        if(inputEvent.inputAxis == EnumAxis.RightJoystickButton) {
            if(inputEvent.value > 0) {
                Reset();
            }
        }
    }

	void Start() {
        if (playerTransform == null) {
            Debug.LogError("No transform is linked to the camera controller");
        }

        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
        distanceDefault = Mathf.Clamp(distanceDefault, distanceMin, distanceMax);
	}
	
	void LateUpdate() {
        if (playerTransform == null)
            return;

        CalculateMovement();
        CalculateCameraTarget();
        UpdatePosition();
	}

    void CalculateMovement() {
        distance -= controllerInput.y * controllerSensitivity.y;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);

        rotation.x += controllerInput.x * controllerSensitivity.x;
        if (rotation.x > 360) {
            rotation.x -= 360;
        }
        else if (rotation.x < 360) {
            rotation.x += 360;
         }
    }

    void CalculateCameraTarget() {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotate = Quaternion.Euler(rotation.y, rotation.x, 0);
        positionTarget = playerTransform.position + rotate * direction;
    }

    void UpdatePosition() {
        var posX = Mathf.Lerp(transform.position.x, positionTarget.x, movementSmooth.x);
        var posY = Mathf.Lerp(transform.position.y, positionTarget.y, movementSmooth.y);
        var posZ = Mathf.Lerp(transform.position.z, positionTarget.z, movementSmooth.z);

        transform.position = new Vector3(posX, posY, posZ);
        transform.LookAt(playerTransform);
    }

    void Reset() {
        distance = distanceDefault;
        rotation = new Vector2(0f, 40f);
    }
}
