using UnityEngine;
using System.Collections;

public class BezierCurveGenerator {

	private int segmentCount;

	private Vector3 startPosition;
	private Vector3 endPosition;
	private	Vector3 startTangent;
	private Vector3 endTangent;

	private Vector3[] bezierPoints;


	public BezierCurveGenerator() { }

	// Generates a bezier curve
	// out curve: The Vector3[] were to output the curve
	// Return: if the curve was regenerated
	public bool GenerateBezierCurve(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int segmentCount, out Vector3[] curve) {
		if(this.segmentCount != segmentCount || this.startPosition != startPosition || this.endPosition != endPosition 
		   || this.startTangent != startTangent || this.endTangent != endTangent) {
			this.segmentCount = segmentCount;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			this.startTangent = startTangent;
			this.endTangent = endTangent;

			Debug.Log("Regenerating curve");

			curve = new Vector3[segmentCount + 1];
			curve[0] = startPosition;
			curve[segmentCount] = endPosition;

			float inc = 1.0F / segmentCount;
			for(int i = 1; i < segmentCount; ++i) {
				curve[i] = CalculateBezierPoint(i * inc);
			}

			bezierPoints = curve;

			return true;
		}

		curve = bezierPoints;

		return false;
	}

	// Calculates a point at a fraction t along the curve
	// [x,y,z]= (1–t)^3 * startPosition + 3 * (1–t)^2 * t * startTangent + 3 * (1–t) * t^2 * endTangent + t^3 * endPosition
	private Vector3 CalculateBezierPoint(float t) {
		float t2 = t * t;
		float t3 = t2 * t;
		float u = 1 - t;
		float u2 = u * u;
		float u3 = u2 * u;

		return (u3 * startPosition) + (3 * u2 * t * startTangent) + (3 * u * t2 * endTangent) + (t3 * endPosition);
	}
}
