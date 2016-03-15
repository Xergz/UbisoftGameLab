using RAIN.Core;
using UnityEngine;

public class RammingEntity : Entity {

	public bool IsRamming { get { return isRamming; } set { isRamming = value; } }


	[Tooltip("Speed to give the fist when moving against a stream. This is to prevent the fist from jamming")]
	public float speedAgainstStream = 8F;
	public float normalSpeed = 4F;


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

	protected override void Start() {
		base.Start();
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
		rigidBody.velocity = Vector3.zero;
	}

    private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.GetComponent<Entity>() != null) {
			collision.gameObject.GetComponent<Entity>().ReceiveHit();
		}
	}

	protected override void OnTriggerStay(Collider other) {
		if(!IsRamming && !isStuned) {
			if(other.gameObject.CompareTag("Stream")) {
				Vector3 force = other.GetComponent<Stream>().GetForceAtPosition(transform.position);
				if(!Mathf.Approximately(force.magnitude, 0F)) {
					if(Vector3.Angle(force, transform.forward) > 90) {
						(tRig.AI.Motor as UnityNavMeshMotor).Speed = speedAgainstStream;
					} else {
						(tRig.AI.Motor as UnityNavMeshMotor).Speed = normalSpeed;
					}
					rigidBody.AddForce(force);
				} else {
					(tRig.AI.Motor as UnityNavMeshMotor).Speed = normalSpeed;
				}
			}
		}
	}
}
