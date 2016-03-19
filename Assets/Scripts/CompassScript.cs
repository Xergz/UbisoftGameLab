using UnityEngine;
using System.Collections;

public class CompassScript : MonoBehaviour {
    
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
        nextFragmentPosition = PlayerController.fragmentsList[PlayerController.nextFragmentIndex].position;

        CalculateVector();

        lastAngle = 0.0F;
    }

    void Update() {
        Debug.Log("index : " + PlayerController.nextFragmentIndex);
        Debug.Log("number of fragment : " + PlayerController.numberOfFragments);
        for (int i = 0; i < PlayerController.fragmentsList.Count; i++)
        {
            if (PlayerController.fragmentsList[i] != null && PlayerController.fragmentsList[i].position == PlayerController.playerRigidbody.position)
                Debug.Log("Position[" + i + "] : " + PlayerController.fragmentsList[i].position + " = player Position");
            else if (PlayerController.fragmentsList[i] != null)
                Debug.Log("Position[" + i + "] : " + PlayerController.fragmentsList[i].position);
            else
                Debug.Log("Position[" + i + "] : null");
        }
        
        nextFragmentPosition = PlayerController.fragmentsList[PlayerController.nextFragmentIndex].position;
        if (PlayerController.nextFragmentIndex < PlayerController.numberOfFragments){
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
        compassDirection.GetComponent<RectTransform>().RotateAround(imageCenter, Vector3.forward, angle);
    }

    public void CalculateVector(){
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
