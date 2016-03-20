using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

    public GameObject point;

    // Update is called once per frame
    void Update () {
        transform.RotateAround(point.transform.position, point.transform.up, 150 * Time.deltaTime);
    }
}
