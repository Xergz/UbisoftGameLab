using RAIN.Core;
using UnityEngine;

public class RammingEntity : Entity {

	public bool IsRamming { get { return isRamming; } set { isRamming = value; } }

    private Rigidbody rigidBody;
    private AIRig tRig;
    private NavMeshAgent navAgent;

    [SerializeField]
	private bool isRamming;

    [SerializeField]
    private bool isStuned;

    [SerializeField]
    private float stunTime = 2f;
    private float beginStunTime;

    [SerializeField]
	private int life = 5;

	private void Start() {
		rigidBody = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        tRig = GetComponentInChildren<AIRig>();
		IsRamming = false;
	}

    private void Update() {
        if (isStuned && Time.time > beginStunTime + stunTime) {
            isStuned = false;
            tRig.AI.IsActive = true;
            navAgent.Resume();
        }
    }

    public override void ReceiveHit() {
		life -= 1;
		if(life <= 0)
			Destroy(gameObject);
	}

    public override void ReceiveStun() {
        isStuned = true;
        beginStunTime = Time.time;
        tRig.AI.IsActive = false;
        navAgent.Stop();
    }

    private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.GetComponent<Entity>() != null) {
			collision.gameObject.GetComponent<Entity>().ReceiveHit();
		}
	}

	protected override void OnTriggerStay(Collider other) {
		if(!IsRamming) {
			if(other.gameObject.CompareTag("Stream")) {
				rigidBody.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
			}
		}
	}
}
