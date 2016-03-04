using UnityEngine;
using RAIN.Core;

public class ChasingEntity : Entity {

    public bool IsDodging { get { return isDodging; } set { isDodging = value; } }

    private Rigidbody rigidBody;
    private AIRig tRig;

    [SerializeField]
    private int life = 5;

    [SerializeField]
    private bool isStuned;

    [SerializeField]
    private float stunTime = 2f;
    private float beginStunTime;

    [SerializeField]
    private bool isDodging;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        tRig = GetComponentInChildren<AIRig>();
        isDodging = false;
    }

    private void update() {
        if(isStuned && Time.time > beginStunTime + stunTime) {
            isStuned = false;
            tRig.AI.IsActive = true;
        }
    }

    public override void ReceiveHit() {
        life -= 1;
        if (life <= 0)
            Destroy(gameObject);
    }

    public override void ReceiveStun() {
        isStuned = true;
        beginStunTime = Time.time;
        tRig.AI.IsActive = false;
    }

    protected override void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Stream"))
            rigidBody.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
    }
}
