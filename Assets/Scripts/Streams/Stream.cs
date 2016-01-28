using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class Stream : MonoBehaviour {

	public int segmentCount = 20;

	public bool debug = false;

	public Material debugMaterial;

	public Vector3 startPosition;
	public Vector3 endPosition;
	public Vector3 startTangent;
	public Vector3 endTangent;


	private LineRenderer lineRenderer;

	private Vector3[] streamCurve;

	private BezierCurveGenerator curveGenerator; 

	void Start () {
		curveGenerator = new BezierCurveGenerator(segmentCount);
		if(debug) {
			lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.SetWidth(0.05F, 0.05F);
			lineRenderer.material = debugMaterial;
			lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
			lineRenderer.receiveShadows = false;
		}
	}
	
	void Update () {
		curveGenerator.SetSegmentCount(segmentCount);
		curveGenerator.SetHandles(startPosition, endPosition, startTangent, endTangent);
		streamCurve = curveGenerator.GenerateBezierCurve();

		if(debug) {
			lineRenderer.SetVertexCount(segmentCount + 1);
			lineRenderer.SetPositions(streamCurve);
		}
	}
}
