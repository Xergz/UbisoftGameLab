using RAIN.Core;
using UnityEngine;

public class RammingEntity : Entity {

	public bool IsRamming { get { return isRamming; } set { isRamming = value; } }

    private Rigidbody rigidBody;
    private AIRig tRig;

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
        tRig = GetComponentInChildren<AIRig>();
		IsRamming = false;
	}

    private void update() {
        if (isStuned && Time.time > beginStunTime + stunTime) {
            isStuned = false;
            tRig.AI.IsActive = true;
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
