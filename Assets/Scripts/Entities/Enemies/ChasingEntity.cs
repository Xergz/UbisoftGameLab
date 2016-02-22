using UnityEngine;
using System.Collections;

public class ChasingEntity : Entity {

    public Rigidbody rigidBody;

    [SerializeField]
    private int life = 5;

    private void Start() {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public override void ReceiveHit() {
        life -= 1;
        if (life <= 0)
            Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other) {
    }

    protected override void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Stream"))
            rigidBody.AddForce(other.gameObject.GetComponent<Stream>().GetForceAtPosition(transform.position));
    }

}
