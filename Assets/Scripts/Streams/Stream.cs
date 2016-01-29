using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class Stream : MonoBehaviour {

	public int segmentCount = 20;

	public float width = 5F;
	public float power = 5F;

	[Tooltip("Displays the starting and ending tangents as well as the curve itself")]
	public bool debug = false;

	public Vector3 startPosition;
	public Vector3 endPosition;
	public Vector3 startTangent;
	public Vector3 endTangent;

	public GameObject tangentArrow;

	public EnumStreamDirection direction = EnumStreamDirection.POSITIVE;


	private const int MAX_SEGMENT_COUNT = 200;
	private const int MIN_SEGMENT_COUNT = 10;

	private const float MAX_CURVE_POINTS_SPACING = 2F;

	private float oldWidth;

	private EnumStreamDirection oldDirection;

	private int[] triangles;

	private Vector3[] streamCurve;
	private Vector3[] tangents;
	private Vector3[] vertices;
	private Vector2[] uv;
	private Vector3[] normals;

	private LineRenderer startLineRenderer, endLineRenderer, curveLineRenderer;
	
	private Mesh mesh;

	private BezierCurveGenerator curveGenerator;

	
	// Return the force to apply to a unit within the stream (assumes the unit is within the stream)
	public Vector3 GetForceAtPosition(Vector3 position) {
		return tangents[GetClosestCurvePointIndex(position)] * power * (int) direction;
	}


	private void Start () {
		curveGenerator = new BezierCurveGenerator();

		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;

		oldWidth = width - 1;
		oldDirection = (EnumStreamDirection) (-((int) direction));

		#region DEBUG
		curveLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
		startLineRenderer = transform.GetChild(2).GetComponent<LineRenderer>();
		endLineRenderer = transform.GetChild(3).GetComponent<LineRenderer>();
		#endregion
	}
	
	private void Update () {
		UpdateCurve();

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

	// Update the stream's curve
	private void UpdateCurve() {
		// Only update the curve if a new one was generated
		segmentCount = Mathf.Max(Mathf.Min(segmentCount, MAX_SEGMENT_COUNT), MIN_SEGMENT_COUNT);
		bool curveChanged = curveGenerator.GenerateBezierCurve(startPosition, endPosition, startTangent, endTangent, segmentCount, out streamCurve);

		if(curveChanged) {
			// To make sure the tangents are accurate enough
			while(Vector3.Distance(streamCurve[0], streamCurve[1]) > MAX_CURVE_POINTS_SPACING && segmentCount < MAX_SEGMENT_COUNT) {
				segmentCount = Mathf.Min(segmentCount + 10, MAX_SEGMENT_COUNT);
				curveGenerator.GenerateBezierCurve(startPosition, endPosition, startTangent, endTangent, segmentCount, out streamCurve);
			}

			// Calculate the tangents for every point
			tangents = new Vector3[streamCurve.Length];
			tangents[0] = Vector3.Normalize(startTangent - startPosition);
			tangents[streamCurve.Length - 1] = Vector3.Normalize(-(endTangent - endPosition));

			for(int i = 1; i < streamCurve.Length - 1; ++i) {
				tangents[i] = Vector3.Normalize(streamCurve[i + 1] - streamCurve[i - 1]);
			}
		}

		if(curveChanged || oldDirection != direction) {
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

			oldDirection = direction;
		}

		if(curveChanged || oldWidth != width) {
			GenerateMesh();
			oldWidth = width;
		}
	}

	// Generate the stream's mesh
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

			vertices[i2] = streamCurve[i] + rotatedTangent * (width / 2);
			vertices[i2 + 1] = streamCurve[i] - rotatedTangent * (width / 2);

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

	// Gets the closest curve point to a position in the world
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
}
