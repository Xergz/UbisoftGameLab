using RAIN.Core;
using UnityEngine;

public class RammingEntity : Entity {

	public bool IsRamming { get { return isRamming; } set { isRamming = value; } }


	[SerializeField]
	private bool isRamming;

	private Rigidbody rigidBody;

	[SerializeField]
	private int life = 5;

	private void Start() {
		rigidBody = GetComponent<Rigidbody>();
		IsRamming = false;
	}

	public override void ReceiveHit() {
		life -= 1;
		if(life <= 0)
			Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.GetComponent<Entity>() != null) {
			collision.gameObject.GetComponent<Entity>().ReceiveHit();

			if(collision.gameObject.CompareTag("Player")) {
				Destroy(gameObject);
			}
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
