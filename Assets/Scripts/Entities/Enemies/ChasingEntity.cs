using UnityEngine;
using System.Collections;

public class ChasingEntity : Entity {

    public bool IsDodging { get { return isDodging; } set { isDodging = value; } }

    private Rigidbody rigidBody;

    [SerializeField]
    private int life = 5;

    [SerializeField]
    private bool isDodging;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        isDodging = false;
    }

    public override void ReceiveHit() {
        life -= 1;
        if (life <= 0)
            Destroy(gameObject);
    }

    protected override void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Stream"))
            rigidBody.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
    }
}
