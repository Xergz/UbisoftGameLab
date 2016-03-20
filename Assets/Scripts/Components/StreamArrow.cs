using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * <summary>
 * Animate stream arrows
 * </summary>
 */
public class StreamArrow : MonoBehaviour {

    /**
     * <summary>
     * Helper class to manage individual arrows in the animation
     * </summary>
     */
    private class Arrow {
        public int KeyIndex;
        public GameObject Object;
        public float travelTimeAcc;

        public Arrow(int index, GameObject obj) {
            this.KeyIndex = index;
            this.Object = obj;
        }

        public void InterpolatePosition(float stepTime, Vector3 startPoint, Vector3 endPoint) {
            // Interpolate position
            Vector3 position = Vector3.Lerp (startPoint, endPoint, this.travelTimeAcc / stepTime);

            // Update position
            this.Object.transform.localPosition = position;
        }

        public void InterpolateRotation(float stepTime, Quaternion startRotation, Quaternion endRotation) {
            Quaternion rotation = Quaternion.Lerp (startRotation, endRotation, this.travelTimeAcc / stepTime);

            // Update rotation
            Vector3 rot = rotation.eulerAngles;
            rot.x = 90;
            rotation.eulerAngles = rot;

            this.Object.transform.localRotation = rotation;
        }

        public void InterpolateAlpha(float stepTime, float startAlpha, float endAlpha) {
            float alpha = Mathf.Lerp (startAlpha, endAlpha, this.travelTimeAcc / stepTime);

            Renderer renderer = this.Object.GetComponent<Renderer> ();
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
            renderer.material = mat;
        }

        public bool CanMoveToNextTarget(float stepTime) {
            return this.travelTimeAcc / stepTime >= 1;
        }

        public void TargetNextKeyFrame(int limit) {
            this.KeyIndex++;

            this.KeyIndex %= (limit - 1);

            this.travelTimeAcc = 0;
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

	private Stream stream;

    private bool firstSet;

    /// <summary>
    /// Describe the root game object for all generated arrows
    /// </summary>
    public GameObject RootObject;

    // Key transform to use while interpolating arrow positions
    private Vector3[] keyPositions;
    private Quaternion[] keyRotations;

	public void SetKeyFrames(Stream attachedStream, Vector3[] keyTransforms, Quaternion[] keyRotations) {
		stream = attachedStream;

        this.keyPositions = keyTransforms;
        this.keyRotations = keyRotations;

        if (firstSet) {
            ArrowCount = keyPositions.Length - 1;

            int arrowIndexStep = keyPositions.Length / ArrowCount;

            arrows = new Arrow[ArrowCount];
            for (int i = 0; i < ArrowCount; i++) {
                arrows [i] = new Arrow (i * arrowIndexStep, (GameObject)Instantiate(this.Prefab));

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

    //
    // UNITY CALLBACKS
    //

    void Start() {
        // If the root game object is not set, the first child is used
        if (RootObject == null) {
            RootObject = transform.GetChild (0).gameObject;
        }

        clearArrows ();
       
        firstSet = true;
    }
        
	// Update is called once per frame
	void Update () {
        if (keyPositions == null)
            return;

        float currentTime = Time.time;
        float dtTime = currentTime - lastTime;

		float speed = ((TravelTime / stream.GetBaseStrength()) * (stream.GetMaxStrength() - stream.GetStrength())) + 1;

        float stepTime = speed / keyPositions.Length;

        // Interpolate positions of every arrows
        foreach(Arrow arrow in arrows) {
            // Interpolate the arrow
            Interpolate(arrow, stepTime);

            // Accumulate elapsed time
            arrow.travelTimeAcc += dtTime;
             
            // If the object has moved to the end, we can move to the next key position
            if (arrow.CanMoveToNextTarget(stepTime)) {
                arrow.TargetNextKeyFrame (keyPositions.Length);
            }
        }
            
        lastTime = currentTime;
	}

    //
    // ARROW INTERPOLATION RELATED HELPERS
    //

    private void Interpolate(Arrow arrow, float stepTime) {
        // Interpolate the position of the arrow
        InterpolatePosition (arrow, stepTime);

        // Interpolate the rotation of the arrow
        InterpolateRotation (arrow, stepTime);

        // Interpolate the fading of the arrow
        InterpolateAlphaFading (arrow, stepTime);
    }

    private void InterpolatePosition(Arrow arrow, float stepTime) {
		// Get start and end positions
		if(arrow.KeyIndex < keyPositions.Length - 1) {
			Vector3 startPoint = keyPositions[arrow.KeyIndex];
			Vector3 endPoint = keyPositions[arrow.KeyIndex + 1];

			arrow.InterpolatePosition(stepTime, startPoint, endPoint);
		}
    }

    private void InterpolateRotation(Arrow arrow, float stepTime) {
		if(arrow.KeyIndex < keyRotations.Length - 1) {
			Quaternion startRotation = keyRotations [arrow.KeyIndex];
			Quaternion endRotation = keyRotations [arrow.KeyIndex + 1];

			arrow.InterpolateRotation (stepTime, startRotation, endRotation);
		}
    }

    private void InterpolateAlphaFading(Arrow arrow, float stepTime) {
        // Fade-in
        if (arrow.KeyIndex == 0) {
            float alphaS = 0;
            float alphaE = 1;

            arrow.InterpolateAlpha (stepTime, alphaS, alphaE);
        } 
        // Fade-out
        else if (arrow.KeyIndex == keyPositions.Length - 2) {
            float alphaS = 1;
            float alphaE = 0;

            arrow.InterpolateAlpha (stepTime, alphaS, alphaE);
        }
    }
}
