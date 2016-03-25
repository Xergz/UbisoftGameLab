﻿using RAIN.Core;
using UnityEngine;

public class RammingEntity : Entity {

	public bool IsRamming { get { return isRamming; } set { isRamming = value; } }


    //[Tooltip("Speed to give the fist when moving against a stream. This is to prevent the fist from jamming")]
    //public float speedAgainstStream = 8F;
    //public float normalSpeed = 4F;

    public GameObject stunStars;

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
            stunStars.SetActive(false);
            navAgent.Resume();
        }
    }

    public override bool ReceiveHit() {
		return false;
	}

    public override void ReceiveStun() {
        isStuned = true;
        beginStunTime = Time.time;
        tRig.AI.IsActive = false;
        stunStars.SetActive(true);
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
					rigidBody.AddForce(force);
				}
			}
		}
	}
}
