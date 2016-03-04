using UnityEngine;
using System.Linq;
using SimplexNoise;
using System.Collections.Generic;

/// <summary>
/// Describes a stream
/// </summary>
[ExecuteInEditMode]
public class Stream : MonoBehaviour {

	/// <summary>
	/// The index of the area the stream is in for the NavMesh
	/// </summary>
	public int AreaIndex { get { return areaIndex; } private set { areaIndex = value; } }

	/// <summary>
	/// The zone in which this stream is physically
	/// </summary>
	public EnumZone Zone { get; set; }

	#region Constants
	private const int MAX_SEGMENT_COUNT = 200; // Maximum number of segments in the bezier curve
	private const int MIN_SEGMENT_COUNT = 10; // Minimum number of segments in the bezier curve
	private const int MAX_WAVE_PRECISION = 15; // Maximum number of points used to simulate the waves
	private const int MIN_WAVE_PRECISION = 3; // Minimum number of points used to simulate the waves

	private const float MAX_CURVE_POINTS_SPACING = 2F; // Maximum distance between 2 points of the curve
	private const float MAX_WIDTH = 1F; // The maximum width for the stream
	private const float MIN_WIDTH = 10F; // The minimum width for the stream 
	private const float MAX_POSITION_AMPLITUDE = 1.75F; // The maximum amplitude for the position oscillation
	private const float MIN_POSITION_AMPLITUDE = 0.25F; // The minimum amplitude for the position oscillation 
	private const float MAX_TANGENT_AMPLITUDE = 25F; // The maximum amplitude for the tangent oscillation
	private const float MIN_TANGENT_AMPLITUDE = 5F; // The minimum amplitude for the tangent oscillation
	private const float MAX_NOISE_OFFSET = 5F; // The maximum noise offset so that not all streams are the same
	private const float MIN_NOISE_OFFSET = 0F; // The minimum noise offset so that not all streams are the same
	private const float MAX_STRENGTH = 10F; // The maximum strength the stream can reach
	private const float MIN_STRENGTH = 1F; // The minimum strength the stream can reach
	#endregion


	[Tooltip("The wave controller to synchronize with")]
	[SerializeField]
	private WaveController waveController;

	[Tooltip("The width of the stream")]
	[Range(MIN_WIDTH, MAX_WIDTH)]
	[SerializeField]
	private float width = 5F;
	// To know when the width changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
	private float oldWidth;
	[Tooltip("The base strength of the stream")]
	[Range(MIN_STRENGTH, MAX_STRENGTH)]
	[SerializeField]
	private float baseStrength = 5F;
	[Tooltip("The current strength of the stream. For in-editor debugging only, do not change this")]
	[SerializeField]
	private float strength;
	[Tooltip("The value that multiplies the strength when calculating an area's cost")]
	[SerializeField]
	private float areaCostModifier = 2;
	[Tooltip("The time before the stream will start gradually going back to its normal strength after the player has modified it (in seconds)")]
	[SerializeField]
	private float timeBeforeStrengthRestoration = 3F;
	[Tooltip("The speed at which the strength will go back to normal (in unit/second)")]
	[SerializeField]
	private float strengthRestorationSpeed = 1F;
	private float timeAtLastStrengthModification = 0F;

	#region Oscillation
	[Tooltip("The speed at which the stream will oscillate")]
	[SerializeField]
	private float oscillationSpeed = 0.25F;
	[Tooltip("The amplitude at which the stream position handles will oscillate in game units")]
	[Range(MIN_POSITION_AMPLITUDE, MAX_POSITION_AMPLITUDE)]
	[SerializeField]
	private float positionOscillationAmplitude = 1F;
	[Tooltip("The amplitude at which the stream tangent handles will oscillate in degrees")]
	[Range(MIN_TANGENT_AMPLITUDE, MAX_TANGENT_AMPLITUDE)]
	[SerializeField]
	private float tangentOscillationAmplitude = 15F;
	#endregion

	[Tooltip("The height of the mesh collider")]
	[SerializeField]
	private float colliderHeight = 4;
	private float randomizedNoiseOffset; // An offset so that not all streams have the same oscillation

	[Tooltip("The exact name of the navigation area this stream is in")]
	[SerializeField]
	private string areaName;

	private int areaIndex; // The index of the area the stream is in for the NavMesh


	[Tooltip("The color of the stream")]
	[SerializeField]
	private EnumStreamColor color = EnumStreamColor.NONE;

	[Tooltip("The direction of the stream")]
	[SerializeField]
	private EnumStreamDirection direction = EnumStreamDirection.POSITIVE;
	// To know when the direction changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
	private EnumStreamDirection oldDirection;


	[Tooltip("Displays the starting and ending tangents as well as the curve itself")]
	[SerializeField]
	private bool debug = false;
	[Tooltip("Whether the stream will oscillate or not")]
	[SerializeField]
	private bool oscillate = true;

	#region Curve Parameters
	[Tooltip("The number of segments in the bezier curve representing the stream")]
	[SerializeField]
	[Range(MIN_SEGMENT_COUNT, MAX_SEGMENT_COUNT)]
	private int segmentCount = 20;
	// To know when the segmentCount changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
	private int oldSegmentCount;
	[Tooltip("The number of point used to simulate the waves. If even, will be augmented by one (sorry don't want to handle even numbers)")]
	[SerializeField]
	[Range(MIN_WAVE_PRECISION, MAX_WAVE_PRECISION)]
	private int wavePrecision = 3;
	// To know when the wave precision changes (needed since I can't do a function if I want to allow in editor modification) I'm open to suggestions
	private int oldWavePrecision;

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

	private Vector3 generalDirection; // The average of all the other tangents
#endregion

	private LineRenderer startLineRenderer, endLineRenderer, curveLineRenderer; // Debug LineRenderers

	#region Meshes
	private int[] colliderTriangles; // The triangles for the collider mesh
	private int[] streamTriangles; // The triangles for the stream mesh

	private Vector3[] streamVertices; // The vertices for the stream mesh

	private Mesh streamMesh; // The curve's mesh. It follows the waves of the ocean
	private Mesh colliderMesh; // The collider mesh. It is tall and does not move.
	private MeshCollider meshCollider;
	#endregion

	#region Materials
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
	#endregion

	[Tooltip("A prefab used to visually represent the direction of the tangents")]
	[SerializeField]
	private GameObject tangentArrow;

	private BezierCurveGenerator curveGenerator; // A bezier curve generator

    [Tooltip("The stream arrow animation controller")]
    [SerializeField]
    private StreamArrow streamArrow = null;

	/// <summary>
	/// Get the force to apply to an object within the stream.
	/// </summary>
	/// <param name="position">The position of the object</param>
	/// <returns>The force to apply to the object. 0 if not in the stream (approximated)</returns>
	public Vector3 GetForceAtPosition(Vector3 position) {
		float distanceToClosest;
		float maxDistanceUpper = 0;
		float maxDistanceLower = 0;
		int closestPoint = GetClosestCurvePointIndex(position, out distanceToClosest);
		// For both end of the curve, we can assume that it's always inside the curve since the collider is precise for this part of the curve.
		Vector3 previousPoint;
		Vector3 currentPoint;
		Vector3 nextPoint;
		if(closestPoint < streamCurve.Length - 1) { // Upper side
			// First side of the stream
			currentPoint = streamVertices[closestPoint * wavePrecision];
			nextPoint = streamVertices[(closestPoint + 1) * wavePrecision];
			maxDistanceUpper = Vector3.Distance((0.5F * (nextPoint - currentPoint)) + currentPoint, streamCurve[closestPoint]);
			// Second side of the stream
			currentPoint = streamVertices[((closestPoint + 1) * wavePrecision) - 1];
			nextPoint = streamVertices[((closestPoint + 2) * wavePrecision) - 1];
			maxDistanceUpper = Mathf.Max(maxDistanceUpper,
										 Vector3.Distance((0.5F * (nextPoint - currentPoint)) + currentPoint, streamCurve[closestPoint]));
		}
		if(closestPoint > 0) { // Lower side
			// First side of the stream
			previousPoint = streamVertices[(closestPoint - 1) * wavePrecision];
			currentPoint = streamVertices[closestPoint * wavePrecision];
			maxDistanceLower = Vector3.Distance((0.5F * (currentPoint - previousPoint)) + previousPoint, streamCurve[closestPoint]);
			// Second side of the stream
			previousPoint = streamVertices[(closestPoint * wavePrecision) - 1];
			currentPoint = streamVertices[((closestPoint + 1) * wavePrecision) - 1];
			maxDistanceLower = Mathf.Max(maxDistanceLower,
										 Vector3.Distance((0.5F * (currentPoint - previousPoint)) + previousPoint, streamCurve[closestPoint]));
		}

		return (distanceToClosest <= Mathf.Max(maxDistanceLower, maxDistanceUpper)) ? tangents[closestPoint] * strength * (int) direction : Vector3.zero;
	}

	/// <summary>
	/// Switch the direction of the stream (POSITIVE -> NEGATIVE or NEGATIVE -> POSITIVE).
	/// </summary>
	public void SwitchDirection() {
		direction = (EnumStreamDirection) (-((int) direction));
	}

	/// <summary>
	/// Increase the strength of the stream
	/// </summary>
	/// <param name="increment">By how much the strength should increase</param>
	public void IncreaseStrength(float increment) {
		strength += increment;
		strength = Mathf.Min(strength, MAX_STRENGTH);
		timeAtLastStrengthModification = Time.time;
	}

	/// <summary>
	/// Decrease the strength of the stream
	/// </summary>
	/// <param name="decrement">By how much the strength should decrease</param>
	public void DecreaseStrength(float decrement) {
		strength -= decrement;
		strength = Mathf.Max(strength, MIN_STRENGTH);
		timeAtLastStrengthModification = Time.time;
	}

	/// <summary>
	/// Sets the handles of the bezier curve representing the stream (positions and tangents) Might be changed to prevent modifiying some values.
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

	/// <summary>
	/// Set the cost of the area linked to this stream according to a vector from position to target
	/// </summary>
	/// <param name="position">The position of the entity willing to set the costs</param>
	/// <param name="target">The target position the entity wishes to reach</param>
	public void SetAreaCost(Vector3 position, Vector3 target) {
		float angle = Vector3.Angle(target - position, generalDirection * (int) direction);
		float delta = strength * areaCostModifier * Mathf.Cos(Mathf.Deg2Rad * angle);
		NavMesh.SetAreaCost(AreaIndex, StreamController.OceanAreaCost - delta);
	}

	/// <summary>
	/// Set the cost of the area linked to this stream to a constant cost
	/// </summary>
	/// <param name="cost">The new cost for the area</param>
	public void SetAreaCost(float cost) {
		NavMesh.SetAreaCost(AreaIndex, cost);
	}


	private void Awake() {
		if(waveController == null) {
			Debug.Log("No WaveController is attached to the stream. Will not follow any waves.");
		}

		curveGenerator = new BezierCurveGenerator();

		streamMesh = new Mesh();
		colliderMesh = new Mesh();
		GetComponent<MeshFilter>().mesh = streamMesh;
		meshCollider = GetComponent<MeshCollider>();

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

		oldWidth = -1F;
		oldSegmentCount = -1;
		oldWavePrecision = -1;
		oldDirection = (EnumStreamDirection) (-((int) direction));
		strength = baseStrength;

		randomizedNoiseOffset = Random.Range(MIN_NOISE_OFFSET, MAX_NOISE_OFFSET);

		AreaIndex = NavMesh.GetAreaFromName(areaName);

		#region DEBUG
		curveLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
		startLineRenderer = transform.GetChild(2).GetComponent<LineRenderer>();
		endLineRenderer = transform.GetChild(3).GetComponent<LineRenderer>();
		#endregion
	}

	private void Start() {
		if(Application.isPlaying) {
			StreamController.RegisterStream(this, color); // We must wait for when the StreamController will be initialized so we use Start
		}
	}

	private void Update() {
		if(!Application.isPlaying) {
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
		}

		// Update the curve
		Vector3 startPosition, startTangent, endPosition, endTangent; // out parameters
		if(oscillate && Application.isPlaying) {
			UpdateHandles(out startPosition, out startTangent, out endPosition, out endTangent);
		} else {
			startPosition = startPositionHandle;
			startTangent = startTangentHandle;
			endPosition = endPositionHandle;
			endTangent = endTangentHandle;
		}
		UpdateCurve(startPosition, startTangent + startPosition, endPosition, endTangent + endPosition);

		// Restore strength
		if(Time.time - timeAtLastStrengthModification >= timeBeforeStrengthRestoration) {
			if(strength > baseStrength) {
				strength = Mathf.Max(baseStrength, strength - (strengthRestorationSpeed * Time.deltaTime));
			} else {
				strength = Mathf.Min(baseStrength, strength + (strengthRestorationSpeed * Time.deltaTime));
			}
		}

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

	private void OnEnable() {
		Awake();
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
		float startNoise = Noise.Generate(1 + randomizedNoiseOffset, Time.time * oscillationSpeed);
		float endNoise = Noise.Generate(segmentCount + 2 + randomizedNoiseOffset, Time.time * oscillationSpeed);

		// Calculate tangents related to original handles (without the oscillation) since the oscillation will be made according to this
		Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.up);
		Vector3 rotatedStartTangent = rotation * Vector3.Normalize(startPositionHandle);
		Vector3 rotatedEndTangent = rotation * Vector3.Normalize(-endPositionHandle);

		// Move the positions along a vector perpendicular to their tangent
		startPosition = startPositionHandle + (rotatedStartTangent * startNoise * positionOscillationAmplitude);
		endPosition = endPositionHandle + (rotatedEndTangent * endNoise * positionOscillationAmplitude);

		// Rotate the tangents around these new positions
		// Therefore move them along with the positions then rotate them around the new positions
		Quaternion startTangentRotation = Quaternion.AngleAxis(startNoise * tangentOscillationAmplitude, Vector3.up);
		Quaternion endTangentRotation = Quaternion.AngleAxis(endNoise * tangentOscillationAmplitude, Vector3.up);
		startTangent = startTangentRotation * startTangentHandle; // Start tangent rotated
		endTangent = endTangentRotation * endTangentHandle; // End tangent rotated
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

		bool generateTriangles = false;

		if(oldWavePrecision != wavePrecision || oldSegmentCount != segmentCount) {
			generateTriangles = true;
			oldWavePrecision = wavePrecision;
			oldSegmentCount = segmentCount;
		}

		if(curveChanged || oldDirection != direction) {
			GenerateTangentArrows();
			oldDirection = direction;
		}

		if(curveChanged || oldWidth != width || waveController != null) {
			GenerateMesh(generateTriangles);
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
		generalDirection = Vector3.zero;
		tangents = new Vector3[streamCurve.Length];

		tangents[0] = Vector3.Normalize(startTangent - startPosition);
		generalDirection += tangents[0];

		tangents[streamCurve.Length - 1] = Vector3.Normalize(-(endTangent - endPosition));
		generalDirection += tangents[streamCurve.Length - 1];

		for(int i = 1; i < streamCurve.Length - 1; ++i) {
			tangents[i] = Vector3.Normalize(streamCurve[i + 1] - streamCurve[i - 1]);
			generalDirection += tangents[i];
		}

		generalDirection = Vector3.Normalize(generalDirection);
	}

	/// <summary>
	/// Generate new arrows to visualize the tangents.
	/// </summary>
	private void GenerateTangentArrows() {
        Vector2[] arrowPositions = new Vector2[streamCurve.Length - 1];
        Quaternion[] arrowRotations = new Quaternion[streamCurve.Length - 1];

        for (int i = 0; i < arrowPositions.Length; ++i) {
            arrowPositions [i] = new Vector2 (streamCurve[i + 1].x, streamCurve[i + 1].z);
            arrowRotations [i] = Quaternion.LookRotation (tangents [i + 1] * (int)direction, Vector3.up);
        }

        if (direction == EnumStreamDirection.NEGATIVE) {
            //arrowPositions = arrowPositions.Reverse ();
            System.Array.Reverse(arrowPositions);
        }

        if (streamArrow) {
            streamArrow.SetKeyFrames(arrowPositions, arrowRotations);
        }
	}

	/// <summary>
	/// Generate the stream's mesh (streamVertices, UV coordinates, normals and triangles).
	/// </summary>
	/// <param name="generateTriangles">Whether we should regenerate the triangles or not. This should only be true if the number of vertices changed.</param>
	private void GenerateMesh(bool generateTriangles) {
		Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.up);
		float halfColliderHeight = colliderHeight / 2;

		wavePrecision = (wavePrecision % 2 == 0) ? wavePrecision - 1 : wavePrecision; // Round down to the nearest odd number

		streamVertices = new Vector3[streamCurve.Length * wavePrecision];
		Vector3[] streamNormals = new Vector3[streamVertices.Length];
		Vector2[] streamUVs = new Vector2[streamVertices.Length];
		Vector3[] colliderVertices = new Vector3[streamCurve.Length << 2];

		// Calculate vertices, UV coordinates and normals for both meshes
		for(int i = 0; i < streamCurve.Length; ++i) {
			int iMultiplied = i * wavePrecision;
			int i4 = i << 2;
			float spacing = width / (wavePrecision - 1);
			Vector3 rotatedTangent = rotation * tangents[i];

			// Vertices (without displacement) and UVs for the stream mesh
			for(int j = 0, orientation = wavePrecision >> 1; j < wavePrecision; ++j, --orientation) {
				streamVertices[iMultiplied + j] = streamCurve[i] + (orientation * spacing * rotatedTangent);
				streamVertices[iMultiplied + j].y = streamVertices[iMultiplied + j].y + 0.01F;
				streamUVs[iMultiplied + j] = new Vector2(streamVertices[iMultiplied + j].x, streamVertices[iMultiplied + j].z);
			}

			// Vertices for the collider mesh
			if(Application.isPlaying) {
				colliderVertices[i4] = streamVertices[iMultiplied];
				colliderVertices[i4].y -= halfColliderHeight;
				colliderVertices[i4 + 1] = streamVertices[iMultiplied + wavePrecision - 1];
				colliderVertices[i4 + 1].y -= halfColliderHeight;
				colliderVertices[i4 + 2] = streamVertices[iMultiplied];
				colliderVertices[i4 + 2].y += halfColliderHeight;
				colliderVertices[i4 + 3] = streamVertices[iMultiplied + wavePrecision - 1];
				colliderVertices[i4 + 3].y += halfColliderHeight;
			}

			// Normals and y displacement for the stream mesh
			if(waveController != null && Application.isPlaying) {
				for(int j = 0; j < wavePrecision; ++j) {
					Vector3 worldVertex = transform.TransformPoint(streamVertices[iMultiplied + j]);
					streamVertices[iMultiplied + j].y = waveController.GetOceanHeightAt(worldVertex.x, worldVertex.z) + 0.1F;
					streamNormals[iMultiplied + j] = waveController.GetSurfaceNormalAt(worldVertex.x, worldVertex.z);
				}
			} else {
				for(int j = 0; j < wavePrecision; ++j) {
					streamNormals[iMultiplied + j] = Vector3.up;
				}
			}
		}

		if(generateTriangles) {
			// Calculate triangles for the stream mesh
			streamTriangles = new int[(streamCurve.Length - 1) * (wavePrecision - 1) * 6];

			for(int i = 0; i < streamCurve.Length - 1; ++i) {
				int iMultiplied = i * wavePrecision;
				int rowIndex = ((iMultiplied - i) * 6);
				for(int j = 0; j < wavePrecision - 1; ++j) {
					int j6 = j * 6;
					int currentIndex = rowIndex + j6;
					int baseVertex = iMultiplied + j;
					streamTriangles[currentIndex] = baseVertex;
					streamTriangles[currentIndex + 1] = baseVertex + wavePrecision;
					streamTriangles[currentIndex + 2] = baseVertex + 1;
					streamTriangles[currentIndex + 3] = baseVertex + 1;
					streamTriangles[currentIndex + 4] = baseVertex + wavePrecision;
					streamTriangles[currentIndex + 5] = baseVertex + 1 + wavePrecision;
				}
			}

			#region Calculate triangles for the collider mesh
			if(oscillate && Application.isPlaying) {
				colliderTriangles = new int[((streamCurve.Length - 1) * 24) + 12];

				// Front
				colliderTriangles[0] = 0;
				colliderTriangles[1] = 2;
				colliderTriangles[2] = 1;
				colliderTriangles[3] = 1;
				colliderTriangles[4] = 2;
				colliderTriangles[5] = 3;
				// Back
				colliderTriangles[colliderTriangles.Length - 6] = colliderVertices.Length - 3;
				colliderTriangles[colliderTriangles.Length - 5] = colliderVertices.Length - 1;
				colliderTriangles[colliderTriangles.Length - 4] = colliderVertices.Length - 4;
				colliderTriangles[colliderTriangles.Length - 3] = colliderVertices.Length - 4;
				colliderTriangles[colliderTriangles.Length - 2] = colliderVertices.Length - 1;
				colliderTriangles[colliderTriangles.Length - 1] = colliderVertices.Length - 2;
				for(int i = 0; i < streamCurve.Length - 1; ++i) {
					int i4 = i << 2;
					int i24 = i * 24;
					// Bottom
					colliderTriangles[i24 + 6] = i4;
					colliderTriangles[i24 + 7] = i4 + 1;
					colliderTriangles[i24 + 8] = i4 + 4;
					colliderTriangles[i24 + 9] = i4 + 4;
					colliderTriangles[i24 + 10] = i4 + 1;
					colliderTriangles[i24 + 11] = i4 + 5;
					// Top
					colliderTriangles[i24 + 12] = i4 + 2;
					colliderTriangles[i24 + 13] = i4 + 6;
					colliderTriangles[i24 + 14] = i4 + 3;
					colliderTriangles[i24 + 15] = i4 + 3;
					colliderTriangles[i24 + 16] = i4 + 6;
					colliderTriangles[i24 + 17] = i4 + 7;
					// Left
					colliderTriangles[i24 + 18] = i4;
					colliderTriangles[i24 + 19] = i4 + 4;
					colliderTriangles[i24 + 20] = i4 + 2;
					colliderTriangles[i24 + 21] = i4 + 2;
					colliderTriangles[i24 + 22] = i4 + 4;
					colliderTriangles[i24 + 23] = i4 + 6;
					// Right
					colliderTriangles[i24 + 24] = i4 + 1;
					colliderTriangles[i24 + 25] = i4 + 3;
					colliderTriangles[i24 + 26] = i4 + 5;
					colliderTriangles[i24 + 27] = i4 + 5;
					colliderTriangles[i24 + 28] = i4 + 3;
					colliderTriangles[i24 + 29] = i4 + 7;
				}
			}
			#endregion
		}

		streamMesh.Clear();
		streamMesh.vertices = streamVertices;
		streamMesh.uv = streamUVs;
		streamMesh.normals = streamNormals;
		streamMesh.triangles = streamTriangles;

		if(Application.isPlaying) {
			colliderMesh.Clear();
			colliderMesh.vertices = colliderVertices;
			colliderMesh.triangles = colliderTriangles;
			meshCollider.sharedMesh = colliderMesh;
		}
	}

	/// <summary>
	/// Get the closest curve point to a position in the world.
	/// </summary>
	/// <param name="position">The position to compare to the curve's points</param>
	/// <returns>The index of the closest curve point</returns>
	private int GetClosestCurvePointIndex(Vector3 position, out float smallestDistance) {
		int closestPoint = 0;

		smallestDistance = Vector3.Distance(position, streamCurve[0]);

		for(int i = 1; i < streamCurve.Length; ++i) {
			float distance = Vector3.Distance(position, streamCurve[i]);

			if(distance < smallestDistance) {
				smallestDistance = distance;
				closestPoint = i;
			}
		}

		return closestPoint;
	}
}
