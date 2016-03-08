using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StreamArrow : MonoBehaviour {
    private class Arrow {
        public int KeyIndex;
        public GameObject Object;
        public float travelTimeAcc;
        public float timeAcc;

        public Arrow(int index, GameObject obj) {
            this.KeyIndex = index;
            this.Object = obj;
        }
    }

    private float lastTime;

    /// <summary>
    /// The number of arrow to display
    /// </summary>
    [Tooltip("The number of arrow to display")]
    [SerializeField]
    public int ArrowCount;

    public GameObject Prefab;
    public float TravelTime = 5.0f;

    private Arrow[] arrows;

    private bool firstSet;

    /// <summary>
    /// Describe the root game object for all generated arrows
    /// </summary>
    public GameObject RootObject;

    // Key transform to use while interpolating arrow positions
    private Vector3[] keyPositions;
    private Quaternion[] keyRotations;

    public void SetKeyFrames(Vector3[] keyTransforms, Quaternion[] keyRotations) {
        this.keyPositions = keyTransforms;
        this.keyRotations = keyRotations;

        if (firstSet) {

            if (ArrowCount > keyPositions.Length - 1) {
                ArrowCount = keyPositions.Length - 1;
            }

            int arrowIndexStep = keyPositions.Length / ArrowCount;

            arrows = new Arrow[ArrowCount];
            for (int i = 0; i < ArrowCount; i++) {
                arrows [i] = new Arrow (i * arrowIndexStep, (GameObject)Instantiate(this.Prefab));

                arrows [i].timeAcc = arrows [i].KeyIndex * (TravelTime / keyPositions.Length);

                arrows [i].Object.transform.parent = RootObject.transform;
            }


            firstSet = false;
        }
    }
        
    void clearArrows() {
        List<Transform> children = transform.GetChild(0).Cast<Transform>().ToList();
        if(Application.isPlaying) {
            foreach(Transform child in children) {
                Destroy(child.gameObject);
            }
        } else {
            foreach(Transform child in children) {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    void Start() {
        clearArrows ();
       
        firstSet = true;
    }

	// Update is called once per frame
	void Update () {
        if (keyPositions == null)
            return;

        float currentTime = Time.time;
        float dtTime = currentTime - lastTime;

        float stepTime = TravelTime / keyPositions.Length;

        // Interpolate positions of every arrows
        foreach(Arrow arrow in arrows) {

            // Get start and end positions
            Vector3 startPoint = keyPositions [arrow.KeyIndex];
            Vector3 endPoint = keyPositions [arrow.KeyIndex + 1];

            Quaternion startRotation = keyRotations [arrow.KeyIndex];
            Quaternion endRotation = keyRotations [arrow.KeyIndex + 1];

            // Interpolate position
            Vector3 position = Vector3.Lerp (startPoint, endPoint, arrow.travelTimeAcc / stepTime);
            Quaternion rotation = Quaternion.Lerp (startRotation, endRotation, arrow.travelTimeAcc / stepTime);

            arrow.Object.transform.localPosition = position;
       
            Vector3 rot = rotation.eulerAngles;
            rot.x = 90;
            rotation.eulerAngles = rot;

            arrow.Object.transform.localRotation = rotation;

            // Accumulate elapsed time
            arrow.travelTimeAcc += dtTime;

            arrow.timeAcc += dtTime;
             
            // If the object has moved to the end, we can move to the next key position
            if (position == endPoint) {
                arrow.KeyIndex++;

                arrow.KeyIndex %= (keyPositions.Length - 1);

                arrow.travelTimeAcc = 0;

            }
        }


        lastTime = currentTime;
	}
}
