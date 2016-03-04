using UnityEngine;
using System.Collections;

public class StunEntity : Entity {

    private Rigidbody rigidBody;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void ReceiveHit() {
    }

    public override void ReceiveStun() {
    }

    protected override void OnTriggerStay(Collider other) {
    }
}
