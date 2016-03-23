using UnityEngine;
using System.Collections;

public class LifePickup : MonoBehaviour {
    [Tooltip("Health point value given by this pickup")]
    public int Value = 1;

    void Update(){
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
}
