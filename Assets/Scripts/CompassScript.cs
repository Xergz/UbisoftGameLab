using UnityEngine;
using System.Collections;

public class CompassScript : MonoBehaviour {

    public GameObject Player;
    public GameObject imageDirectionCenter;

    public GameObject compassDirection;
    public Vector3 imageCenter;
    public Vector3 playerPosition;
    public Vector3 nextFragmentPosition;

    public Vector3 playerForwardVector;
    public Vector3 playerFragmentVector;

    public float lastAngle;
    public float currentAngle;

    void Start() {
        imageCenter = imageDirectionCenter.transform.position;
        playerPosition = PlayerController.playerRigidbody.position;
        nextFragmentPosition = PlayerController.nextFragment.position;

        CalculateVector();

        lastAngle = 0.0F;
    }

    void Update() {
        nextFragmentPosition = PlayerController.nextFragment.position;
        if (PlayerController.nextFragment != null){
            CalculateVector();
            RotateDirection(CalculateAngle() - lastAngle);
            lastAngle = currentAngle;

            playerPosition = PlayerController.playerRigidbody.position;
        }
        else {
            RotateDirection(-lastAngle);
            lastAngle = 0.0F;
        }
    }

    void RotateDirection(float angle){
        compassDirection.GetComponent<RectTransform>().RotateAround( imageCenter, Vector3.forward, angle);
    }

    public void CalculateVector(){
        playerForwardVector = Player.GetComponent<Transform>().forward;
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
