using UnityEngine;
using RAIN.Core;
using System.Collections;

public class ChasingEntity : Entity {

    public bool IsDodging { get { return isDodging; } set { isDodging = value; } }

	public float minTimeBetweenAudio = 10F;
	public float maxTimeBetweenAudio = 20F;
	public float distanceForCloseAudio = 20F;
	public float distanceForFarAudio = 40F;

	public AudioController audioController;

    private Rigidbody rigidBody;
    private AIRig tRig;
    private NavMeshAgent navAgent;

    [SerializeField]
    private int life = 5;

    [SerializeField]
    private bool isStuned;

    [SerializeField]
    private float stunTime = 2f;
    private float beginStunTime;

    [SerializeField]
    private bool isDodging;

    protected override void Start() {
		base.Start();
        rigidBody = GetComponent<Rigidbody>();
        tRig = GetComponentInChildren<AIRig>();
        navAgent = GetComponent<NavMeshAgent>();
        isDodging = false;

		StartCoroutine(PlayAudio());
    }

	private IEnumerator PlayAudio() {
		yield return new WaitForSeconds(Random.Range(5F, maxTimeBetweenAudio));

		while(true) {
			if(Distance < distanceForCloseAudio) {
				audioController.PlayAudioClose();
			} else if(Distance < distanceForFarAudio) {
				audioController.PlayAudioFar();
			}

			yield return new WaitForSeconds(Random.Range(minTimeBetweenAudio, maxTimeBetweenAudio));
		}
	}

    private void Update() {
        if(isStuned && Time.time > beginStunTime + stunTime) {
            isStuned = false;
            tRig.AI.IsActive = true;
            navAgent.Resume();
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
        navAgent.Stop();
    }

    protected override void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Stream"))
            rigidBody.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
    }
}
