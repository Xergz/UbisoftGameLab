using UnityEngine;
using System.Collections;

public class MapScript : MonoBehaviour {

    public GameObject playerArrow;
    public Camera mapCamera;
    

    public Transform Target;
    public Vector3 vectorForward = -new Vector3(Vector3.forward.x, 0, Vector3.forward.z);
    public float lastAngle;
    public float currentAngle;

    void LateUpdate() {
        CalculateAngle();
        playerArrow.transform.RotateAround(transform.position, transform.forward, currentAngle - lastAngle);
    }

    public float CalculateAngle()
    {
        lastAngle = currentAngle;
        currentAngle = Vector3.Angle(vectorForward, Target.transform.forward);
        if (Vector3.Cross(Vector3.forward, Target.transform.forward).y < 0)
        {
            currentAngle = -currentAngle;
        }
        return currentAngle;
    }
}
