using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

	public float Distance { get; protected set; }

	public GameObject player { get; set; }

	[Tooltip("The time between each check for the distance between the player and the entity (in seconds)")]
	[SerializeField]
	private float timeBetweenDistanceChecks = 1F;

	[Tooltip("Displays the path of the entity if the pathfinder found one")]
	[SerializeField]
	protected bool debug = false;

	protected bool wasDebugging = false;


    public abstract void ReceiveHit();

    public abstract void ReceiveStun();

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


	protected virtual void Start() {
		player = FindObjectOfType<PlayerController>().Player;

		StartCoroutine(CheckDistanceToPlayer(timeBetweenDistanceChecks));
	}

    protected abstract void OnTriggerStay(Collider other);

	protected virtual void SetupLineRenderer() {
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

	protected virtual IEnumerator CheckDistanceToPlayer(float time) {
		while(true) {
			Distance = Vector3.Distance(player.transform.position, transform.position);

			yield return new WaitForSeconds(time);
		}
	}
}
