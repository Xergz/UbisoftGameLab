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
    private Vector3 movementVelocity = Vector3.zero;


    public override void ReceiveInputEvent(InputEvent inputEvent) {
        if(inputEvent.InputAxis == EnumAxis.RightJoystickX) {
            controllerInput.x = inputEvent.Value;
            if(Mathf.Abs(controllerInput.x) < 0.2) {
                controllerInput.x = 0;
            }
        }

        if(inputEvent.InputAxis == EnumAxis.RightJoystickY) {
            controllerInput.y = inputEvent.Value*-1;
            if(Mathf.Abs(controllerInput.y) < 0.2) {
                controllerInput.y = 0;
            }
        }

        if(inputEvent.InputAxis == EnumAxis.RightJoystickButton) {
            if(inputEvent.Value > 0) {
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
        var posX = Mathf.SmoothDamp(transform.position.x, positionTarget.x, ref movementVelocity.x, movementSmooth.x);
        var posY = Mathf.SmoothDamp(transform.position.y, positionTarget.y, ref movementVelocity.y, movementSmooth.y);
        var posZ = Mathf.SmoothDamp(transform.position.z, positionTarget.z, ref movementVelocity.z, movementSmooth.z);

        transform.position = new Vector3(posX, posY, posZ);
        transform.LookAt(playerTransform);
    }

    void Reset() {
        distance = distanceDefault;
        rotation = new Vector2(0f, 40f);
    }
}
