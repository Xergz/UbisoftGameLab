using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

	[Tooltip("Displays the path of the entity if the pathfinder found one")]
	[SerializeField]
	protected bool debug = false;

	private bool wasDebugging = false;


    public abstract void ReceiveHit();

	public void DrawPath() {
		if(debug) {
			SetupLineRenderer();
			wasDebugging = true;
		} else if(wasDebugging) {
			if(GetComponent<LineRenderer>() != null) {
				GetComponent<LineRenderer>().enabled = false;
			}
			wasDebugging = false;
		}
	}


    protected abstract void OnTriggerStay(Collider other);

	protected void SetupLineRenderer() {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		if(agent != null) {
			LineRenderer line = GetComponent<LineRenderer>();
			if(line == null) {
				line = gameObject.AddComponent<LineRenderer>();
				line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
				line.SetWidth(0.5f, 0.5f);
				line.SetColors(Color.yellow, Color.yellow);
			}

			NavMeshPath path = agent.path;

			if(path != null) {
				line.SetVertexCount(path.corners.Length);
				line.SetPositions(path.corners);
			}
		}
	}
}
