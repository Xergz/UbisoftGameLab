using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using SimplexNoise;

/// <summary>
/// Describes a stream
/// </summary>
public class Stream : MonoBehaviour {

    public EnumStreamColor Color { get { return color; } }

	private const int MAX_SEGMENT_COUNT = 200; // Maximum number of segments in the bezier curve
	private const int MIN_SEGMENT_COUNT = 10; // Minimum number of segments in the bezier curve

	private const float MAX_CURVE_POINTS_SPACING = 2F; // Maximum distance between 2 points of the curve
    private const float MAX_POSITION_AMPLITUDE = 1.75F; // The maximum amplitude for the position oscillation
    private const float MIN_POSITION_AMPLITUDE = 0.25F; // The minimum amplitude for the position oscillation 
	private const float MAX_TANGENT_AMPLITUDE = 25F; // The maximum amplitude for the tangent oscillation
	private const float MIN_TANGENT_AMPLITUDE = 5F; // The minimum amplitude for the tangent oscillation
    private const float RANDOMIZATION_TIME = 30F; // The number of seconds between each randomization of the curve amplitude

    [Tooltip("The width of the stream")]
    [SerializeField]
    private float width = 5F;
    [Tooltip("The strength of the stream")]
    [SerializeField]
    private float strength = 5F;
    // To know when the width changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
    private float oldWidth;
	[Tooltip("The speed at which the stream will oscillate")]
	[SerializeField]
	private float oscillationSpeed = 0.25F;
	[Tooltip("The amplitude at which the stream position handles will oscillate in game units")]
	[SerializeField]
	private float positionOscillationAmplitude = 1F;
	[Tooltip("The amplitude at which the stream tangent handles will oscillate in degrees")]
	[SerializeField]
	private float tangentOscillationAmplitude = 15F;


    [Tooltip("The color of the stream")]
    [SerializeField]
    private EnumStreamColor color = EnumStreamColor.GREEN;

    [Tooltip("The direction of the stream")]
    [SerializeField]
    private EnumStreamDirection direction = EnumStreamDirection.POSITIVE;
    // To know when the direction changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
    private EnumStreamDirection oldDirection;

    [Tooltip("Displays the starting and ending tangents as well as the curve itself")]
    [SerializeField]
    private bool debug = false;

    [Tooltip("The number of segments in the bezier curve representing the stream")]
    [SerializeField]
    [Range(10, 200)]
	private int segmentCount = 20;

    [Tooltip("Starting position of the bezier curve representing the stream")]
    [SerializeField]
    private Vector3 startPositionHandle;
    [Tooltip("Starting tangent of the bezier curve representing the stream")]
    [SerializeField]
    private Vector3 startTangentHandle;
    [Tooltip("Ending position of the bezier curve representing the stream")]
    [SerializeField]
    private Vector3 endPositionHandle;
    [Tooltip("Ending tangent of the bezier curve representing the stream")]
    [SerializeField]
    private Vector3 endTangentHandle;

    private Vector3[] streamCurve; // The bezier curve
	private Vector3[] tangents; // The array containing the tangents for each point of the stream
	private Vector3[] vertices; // The vertices for the stream mesh
	private Vector2[] uv; // The UV coordinates for each vertices
	private Vector3[] normals; // The normals for each vertices

    private int[] triangles; // The triangles for the mesh

    private LineRenderer startLineRenderer, endLineRenderer, curveLineRenderer; // Debug LineRenderers

	private Mesh mesh; // The curve's mesh

	[Tooltip("The material to apply to a green stream")]
    [SerializeField]
    private Material greenStreamMaterial;
	[Tooltip("The material to apply to a blue stream")]
    [SerializeField]
    private Material blueStreamMaterial;
	[Tooltip("The material to apply to a yellow stream")]
    [SerializeField]
    private Material yellowStreamMaterial;
	[Tooltip("The material to apply to a red stream")]
    [SerializeField]
    private Material redStreamMaterial;

    [Tooltip("A prefab used to visually represent the direction of the tangents")]
    [SerializeField]
    private GameObject tangentArrow;

    private BezierCurveGenerator curveGenerator; // A bezier curve generator


    /// <summary>
    /// Get the force to apply to an object within the stream (assumes the object is within the stream).
    /// </summary>
    /// <param name="position">The position of the object</param>
    /// <returns>The force to apply to the object</returns>
    public Vector3 GetForceAtPosition(Vector3 position) {
		return tangents[GetClosestCurvePointIndex(position)] * strength * (int) direction;
	}

    /// <summary>
    /// Switch the direction of the stream (POSITIVE -> NEGATIVE or NEGATIVE -> POSITIVE).
    /// </summary>
    public void SwitchDirection() {
        direction = (EnumStreamDirection) (-((int) direction));
    }

    /// <summary>
    /// Sets the handles of the bezier curve representing the stream (positions and tangents) Might be change to prevent modifiying some values.
    /// </summary>
    /// <param name="startPositionHandle">Starting position of the bezier curve representing the stream</param>
    /// <param name="startTangentHandle">Starting tangent of the bezier curve representing the stream</param>
    /// <param name="endPositionHandle">Ending position of the bezier curve representing the stream</param>
    /// <param name="endTangentHandle">Ending tangent of the bezier curve representing the stream</param>
	public void SetHandles(Vector3 startPositionHandle, Vector3 startTangentHandle, Vector3 endPositionHandle, Vector3 endTangentHandle) {
		this.startPositionHandle = startPositionHandle;
		this.startTangentHandle = startTangentHandle;
		this.endPositionHandle = endPositionHandle;
		this.endTangentHandle = endTangentHandle;
    }


	private void Awake () {
		curveGenerator = new BezierCurveGenerator();

		mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;

        switch(color) {
            case EnumStreamColor.GREEN:
                GetComponent<MeshRenderer>().material = greenStreamMaterial;
                break;
            case EnumStreamColor.BLUE:
                GetComponent<MeshRenderer>().material = blueStreamMaterial;
                break;
            case EnumStreamColor.YELLOW:
                GetComponent<MeshRenderer>().material = yellowStreamMaterial;
                break;
            case EnumStreamColor.RED:
                GetComponent<MeshRenderer>().material = redStreamMaterial;
                break;
        }

        oldWidth = width - 1;
        oldDirection = (EnumStreamDirection) (-((int) direction));

        // Invoke the RandomizeAmplitude method every RANDOMIZATION_TIME
        //InvokeRepeating("RandomizeAmplitude", RANDOMIZATION_TIME, RANDOMIZATION_TIME);

        #region DEBUG
        curveLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
		startLineRenderer = transform.GetChild(2).GetComponent<LineRenderer>();
		endLineRenderer = transform.GetChild(3).GetComponent<LineRenderer>();
		#endregion
	}
	
	private void Update () {
		Vector3 startPosition, startTangent, endPosition, endTangent; // out parameters
		UpdateHandles(out startPosition, out startTangent, out endPosition, out endTangent);
		UpdateCurve(startPosition, startTangent, endPosition, endTangent);

		#region DEBUG
		if(debug) {
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(2).gameObject.SetActive(true);
			transform.GetChild(3).gameObject.SetActive(true);

			curveLineRenderer.SetVertexCount(streamCurve.Length);
			Vector3[] positions = streamCurve.Clone() as Vector3[];
			for(int i = 0; i < positions.Length; ++i) {
				positions[i].y += 0.075F;
			}
			curveLineRenderer.SetPositions(positions);

			startLineRenderer.SetVertexCount(2);
			startLineRenderer.SetPositions(new Vector3[] { new Vector3(startPosition.x, startPosition.y + 0.075F, startPosition.z), new Vector3(startTangent.x, startTangent.y + 0.075F, startTangent.z) });
			
			endLineRenderer.SetVertexCount(2);
			endLineRenderer.SetPositions(new Vector3[] { new Vector3(endPosition.x, endPosition.y + 0.075F, endPosition.z), new Vector3(endTangent.x, endTangent.y + 0.075F, endTangent.z) });
		} else {
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).gameObject.SetActive(false);
		}
		#endregion
	}

	/// <summary>
	/// Update the handles of the curve in order to make them oscillate according to a Perlin noise. 
	/// Position handles oscillate on an axis perpendicular to their tangent.
	/// Tangent handles oscillate on a circular axis around the position handles.
	/// </summary>
	/// <param name="startPosition">Out: The displaced starting position</param>
	/// <param name="startTangent">Out: The displaced starting tangent</param>
	/// <param name="endPosition">Out: The displaced ending position</param>
	/// <param name="endTangent">Out: The displaced ending tangent</param>
	private void UpdateHandles(out Vector3 startPosition, out Vector3 startTangent, out Vector3 endPosition, out Vector3 endTangent) {
		// Get the noise to modify handles' positions
		float startNoise = Noise.Generate(1, Time.time * oscillationSpeed, 1); // Using 3 parameters since the function with 2 is buggy
		float endNoise = Noise.Generate(segmentCount + 2, Time.time * oscillationSpeed, 1); // Using 3 parameters since the function with 2 is buggy

		// Calculate tangents related to original handles (without the oscillation) since the oscillation will be made according to this
		Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.up);
		Vector3 rotatedStartTangent = rotation * Vector3.Normalize(startTangentHandle - startPositionHandle);
		Vector3 rotatedEndTangent = rotation * Vector3.Normalize(-(endTangentHandle - endPositionHandle));

		// Move the positions along a vector perpendicular to their tangent
		startPosition = startPositionHandle + (rotatedStartTangent * startNoise * positionOscillationAmplitude);
		endPosition = endPositionHandle + (rotatedEndTangent * endNoise * positionOscillationAmplitude);

		// Rotate the tangents around these new positions
		// Therefore move them along with the positions then rotate them around the new positions
		Quaternion startTangentRotation = Quaternion.AngleAxis(startNoise * tangentOscillationAmplitude, Vector3.up);
		Quaternion endTangentRotation = Quaternion.AngleAxis(endNoise * tangentOscillationAmplitude, Vector3.up);
		startTangent = (startTangentHandle + (startPositionHandle - startPosition)); // New start tangent position before rotation
		startTangent = (startTangentRotation * (startTangent - startPosition)) + startPosition; // Start tangent rotated
		endTangent = (endTangentHandle + (endPositionHandle - endPosition)); // New end tangent position before rotation
		endTangent = (endTangentRotation * (endTangent - endPosition)) + endPosition; // End tangent rotated
	}

    /// <summary>
    /// Update the stream's bezier curve. Will do nothing if the stream hasn't changed. 
	/// Most of the time the stream has changed due to oscillation.
    /// Can be safely called every frame without impact on performance. 
    /// Also makes sure the curve is smooth enough by calculated the required segment count.
    /// </summary>
	/// <param name="startPosition">Starting position of the bezier curve representing the stream</param>
	/// <param name="startTangent">Starting tangent of the bezier curve representing the stream</param>
	/// <param name="endPosition">Ending position of the bezier curve representing the stream</param>
	/// <param name="endTangent">Ending tangent of the bezier curve representing the stream</param>
	private void UpdateCurve(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent) {
		// Only update the curve if a new one was generated
		segmentCount = Mathf.Max(Mathf.Min(segmentCount, MAX_SEGMENT_COUNT), MIN_SEGMENT_COUNT);
		bool curveChanged = curveGenerator.GenerateBezierCurve(startPosition, startTangent, endPosition, endTangent, segmentCount, out streamCurve);

		if(curveChanged) {
			// To make sure the tangents are accurate enough
			while(Vector3.Distance(streamCurve[0], streamCurve[1]) > MAX_CURVE_POINTS_SPACING && segmentCount < MAX_SEGMENT_COUNT) {
				segmentCount = Mathf.Min(segmentCount + 10, MAX_SEGMENT_COUNT);
				curveGenerator.GenerateBezierCurve(startPosition, startTangent, endPosition, endTangent, segmentCount, out streamCurve);
			}

			GenerateTangents(startPosition, startTangent, endPosition, endTangent);
		}

		if(curveChanged || oldDirection != direction) {
            GenerateTangentArrows();
			oldDirection = direction;
		}

		if(curveChanged || oldWidth != width) {
			GenerateMesh();
			oldWidth = width;
		}
	}

	/// <summary>
	/// Generate new tangents from the curve.
	/// </summary>
	/// <param name="startPosition">Starting position of the bezier curve representing the stream</param>
	/// <param name="startTangent">Starting tangent of the bezier curve representing the stream</param>
	/// <param name="endPosition">Ending position of the bezier curve representing the stream</param>
	/// <param name="endTangent">Ending tangent of the bezier curve representing the stream</param>
	private void GenerateTangents(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent) {
		tangents = new Vector3[streamCurve.Length];
		tangents[0] = Vector3.Normalize(startTangent - startPosition);
		tangents[streamCurve.Length - 1] = Vector3.Normalize(-(endTangent - endPosition));

		for(int i = 1; i < streamCurve.Length - 1; ++i) {
			tangents[i] = Vector3.Normalize(streamCurve[i + 1] - streamCurve[i - 1]);
		}
	}

    /// <summary>
    /// Generate new arrows to visualize the tangents.
    /// </summary>
    private void GenerateTangentArrows() {
        // Clear the old arrows
        foreach(Transform child in transform.GetChild(0)) {
            GameObject.Destroy(child.gameObject);
        }

        // Instantiate the tangent arrows
        for(int i = 1; i < streamCurve.Length - 1; ++i) {
            GameObject arrow = Instantiate(tangentArrow);
            arrow.transform.parent = transform.GetChild(0);
            arrow.transform.position = new Vector3(streamCurve[i].x, streamCurve[i].y + 0.05F, streamCurve[i].z);
            arrow.transform.rotation = Quaternion.LookRotation(tangents[i] * (int) direction, Vector3.up) * arrow.transform.rotation;
        }
    }

	/// <summary>
    /// Generate the stream's mesh (vertices, UV coordinates, normals and triangles).
    /// </summary>
	private void GenerateMesh() {
		Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.up);

		vertices = new Vector3[streamCurve.Length * 2];
		uv = new Vector2[vertices.Length];
		normals = new Vector3[vertices.Length];
		triangles = new int[(streamCurve.Length - 1) * 6];

		// Calculate vertices, UV coordinates and normals
		for(int i = 0; i < streamCurve.Length; ++i) {
			int i2 = i * 2;
			Vector3 rotatedTangent = rotation * tangents[i];

			vertices[i2] = streamCurve[i] + (rotatedTangent * (width / 2));
			vertices[i2 + 1] = streamCurve[i] - (rotatedTangent * (width / 2));

			uv[i2] = new Vector2(vertices[i2].x, vertices[i2].z);
			uv[i2 + 1] = new Vector2(vertices[i2 + 1].x, vertices[i2 + 1].z);

			normals[i2] = Vector3.up;
			normals[i2 + 1] = Vector3.up;
		}

		// Calculate triangles
		for(int i = 0; i < streamCurve.Length - 1; ++i) {
			int i2 = i * 2;
			int i6 = i * 6;
			triangles[i6] = i2;
			triangles[i6 + 1] = i2 + 2;
			triangles[i6 + 2] = i2 + 1;
			triangles[i6 + 3] = i2 + 1;
			triangles[i6 + 4] = i2 + 2;
			triangles[i6 + 5] = i2 + 3;
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.triangles = triangles;
	}

	/// <summary>
    /// Get the closest curve point to a position in the world.
    /// </summary>
	private int GetClosestCurvePointIndex(Vector3 position) {
		int closestPoint = 0;

		float smallestDistance = Vector3.Distance(position, streamCurve[0]);

		for(int i = 1; i < streamCurve.Length; ++i) {
			float distance = Vector3.Distance(position, streamCurve[i]);

			if(distance < smallestDistance) {
				smallestDistance = distance;
				closestPoint = i;
			}
		}

		return closestPoint;
	}

	/// <summary>
	/// Randomize the oscillation amplitudes for the positions and the tangents.
	/// </summary>
    private void RandomizeAmplitude() {
        positionOscillationAmplitude = Random.Range(MIN_POSITION_AMPLITUDE, MAX_POSITION_AMPLITUDE);
		tangentOscillationAmplitude = Random.Range(MIN_TANGENT_AMPLITUDE, MAX_TANGENT_AMPLITUDE);
    }
}
