using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class Stream : MonoBehaviour {

	public int segmentCount = 20;
	public float width = 5F;

	public bool debug = false;

	public Vector3 startPosition;
	public Vector3 endPosition;
	public Vector3 startTangent;
	public Vector3 endTangent;

	private LineRenderer startLineRenderer, endLineRenderer;
	
	private Mesh mesh;

	[SerializeField]
	private Vector3[] streamCurve;
	[SerializeField]
	private Vector3[] tangents;
	[SerializeField]
	private Vector3[] vertices;
	[SerializeField]
	private Vector2[] uv;
	[SerializeField]
	private Vector3[] normals;
	[SerializeField]
	private int[] triangles;

	private BezierCurveGenerator curveGenerator;

	void Start () {
		curveGenerator = new BezierCurveGenerator();
		mesh = new Mesh();

		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;

		if(debug) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
			startLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
			endLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
		} else {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
		}
	}
	
	void Update () {
		UpdateCurve();

		if(debug) {
			startLineRenderer.SetVertexCount(2);
			startLineRenderer.SetPositions(new Vector3[] { startPosition, startTangent });

			endLineRenderer.SetVertexCount(2);
			endLineRenderer.SetPositions(new Vector3[] { endPosition, endTangent });
		}
	}

	private void UpdateCurve() {
		streamCurve = curveGenerator.GenerateBezierCurve(startPosition, endPosition, startTangent, endTangent, Mathf.Max(segmentCount, 10));

		// To make sure the tangent are accurate enough
		while(Vector3.Distance(streamCurve[0], streamCurve[1]) > 2) {
			segmentCount += 10;
			streamCurve = curveGenerator.GenerateBezierCurve(startPosition, endPosition, startTangent, endTangent, segmentCount);
		}

		// Calculate the tangents for every point
		tangents = new Vector3[streamCurve.Length];
		tangents[0] = Vector3.Normalize(startTangent - startPosition);
		tangents[streamCurve.Length - 1] = Vector3.Normalize(-(endTangent - endPosition));

		for(int i = 1; i < streamCurve.Length - 1; ++i) {
			tangents[i] = Vector3.Normalize(streamCurve[i + 1] - streamCurve[i - 1]);
		}

		// Calculate vertices
		Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.up);

		vertices = new Vector3[streamCurve.Length * 2];
		uv = new Vector2[vertices.Length];
		normals = new Vector3[vertices.Length];
		triangles = new int[(streamCurve.Length - 1) * 6];

		for(int i = 0; i < streamCurve.Length; ++i) {
			Vector3 rotatedTangent = rotation * tangents[i];

			vertices[i * 2] = streamCurve[i] + rotatedTangent * (width / 2);
			vertices[(i * 2) + 1] = streamCurve[i] - rotatedTangent * (width / 2);

			uv[i * 2] = new Vector2(vertices[i * 2].x, vertices[i * 2].z);
			uv[(i * 2) + 1] = new Vector2(vertices[(i * 2) + 1].x, vertices[(i * 2) + 1].z);

			normals[i * 2] = Vector3.up;
			normals[(i * 2) + 1] = Vector3.up;
		}

		for(int i = 0; i < streamCurve.Length - 1; ++i) {
			triangles[i * 6] = (i * 2);
			triangles[(i * 6) + 1] = (i * 2) + 2;
			triangles[(i * 6) + 2] = (i * 2) + 1;
			triangles[(i * 6) + 3] = (i * 2) + 1;
			triangles[(i * 6) + 4] = (i * 2) + 2;
			triangles[(i * 6) + 5] = (i * 2) + 3;
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.triangles = triangles;
	}
}
