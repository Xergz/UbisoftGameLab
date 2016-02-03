using UnityEngine;
using System.Collections;

/// <summary>
/// A class generating a bezier curve and regenerating it when it changes
/// </summary>
public class BezierCurveGenerator {

	private int segmentCount;

	private Vector3 startPosition;
    private Vector3 startTangent;
    private Vector3 endPosition;
	private Vector3 endTangent;

	private Vector3[] bezierPoints;


	public BezierCurveGenerator() { }

    /// <summary>
    /// Generates a bezier curve
    /// </summary>
    /// <param name="startPosition">Starting position of the bezier curve representing the stream</param>
    /// <param name="startTangent">Starting tangent of the bezier curve representing the stream</param>
    /// <param name="endPosition">Ending position of the bezier curve representing the stream</param>
    /// <param name="endTangent">Ending tangent of the bezier curve representing the stream</param>
    /// <param name="segmentCount">The number of curve points to generate</param>
    /// <param name="curve">Out param: The resulting array of curve points</param>
    /// <returns>Whether the curve was regenerated or not</returns>
    public bool GenerateBezierCurve(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, int segmentCount, out Vector3[] curve) {
		if(this.segmentCount != segmentCount 
           || this.startPosition != startPosition || this.startTangent != startTangent
           || this.endPosition != endPosition || this.endTangent != endTangent) {
			this.segmentCount = segmentCount;
			this.startPosition = startPosition;
            this.startTangent = startTangent;
            this.endPosition = endPosition;
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

    /// <summary>
    /// Calculates a point at a percentage t along the curve.
    /// Uses this equation:
    /// [x,y,z]= (1–t)^3 * startPosition + 3 * (1–t)^2 * t * startTangent + 3 * (1–t) * t^2 * endTangent + t^3 * endPosition
    /// </summary>
    /// <param name="t">The percentage of the curve at which the point to generate stands</param>
    private Vector3 CalculateBezierPoint(float t) {
		float t2 = t * t;
		float t3 = t2 * t;
		float u = 1 - t;
		float u2 = u * u;
		float u3 = u2 * u;

		return (u3 * startPosition) + (3 * u2 * t * startTangent) + (3 * u * t2 * endTangent) + (t3 * endPosition);
	}
}
