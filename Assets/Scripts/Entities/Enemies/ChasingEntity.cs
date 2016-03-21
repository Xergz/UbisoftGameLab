using UnityEngine;
using RAIN.Core;
using System.Collections;

public class ChasingEntity : Entity {

    public bool IsDodging { get { return isDodging; } set { isDodging = value; } }

	public float minTimeBetweenAudio = 10F;
	public float maxTimeBetweenAudio = 20F;
	public float distanceForCloseAudio = 20F;
	public float distanceForFarAudio = 40F;
    //[Tooltip("Speed to give the shade when moving against a stream. This is to prevent the shade from jamming")]
    //public float speedAgainstStream = 8F;
    //public float normalSpeed = 4F;

    public GameObject stunStars;

	public GameObject meshObject;

	public ParticleSystem dieParticles;


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
				audioController.PlayAudio(AudioController.soundType.close);
            } else if(Distance < distanceForFarAudio) {
				audioController.PlayAudio(AudioController.soundType.far);
            }

			yield return new WaitForSeconds(Random.Range(minTimeBetweenAudio, maxTimeBetweenAudio));
		}
	}

    private void Update() {
        if(isStuned && Time.time > beginStunTime + stunTime) {
            isStuned = false;
            tRig.AI.IsActive = true;
            stunStars.SetActive(false);
            navAgent.Resume();
        }
    }

    public override void ReceiveHit() {
        life -= 1;
        audioController.PlayAudio(AudioController.soundType.receiveHit);
        if (life <= 0) {
			SetDying();
		}
    }

    public override void ReceiveStun() {
        audioController.PlayAudio(AudioController.soundType.receiveStun);
        isStuned = true;
        beginStunTime = Time.time;
        tRig.AI.IsActive = false;
        stunStars.SetActive(true);

        navAgent.Stop();
		rigidBody.velocity = Vector3.zero;
    }

	protected override void OnTriggerStay(Collider other) {
		if(!isStuned && !isDodging) {
			if(other.gameObject.CompareTag("Stream")) {
				Vector3 force = other.GetComponent<Stream>().GetForceAtPosition(transform.position);
				if(!Mathf.Approximately(force.magnitude, 0F)) {
					rigidBody.AddForce(force);
				}
			}
		}
	}

	public void SetDying() {
		meshObject.SetActive(false);
		dieParticles.Emit(150);
		StartCoroutine(DestroyInXSeconds(2));
	}

	private IEnumerator DestroyInXSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);

		Destroy(gameObject);
	}
}
