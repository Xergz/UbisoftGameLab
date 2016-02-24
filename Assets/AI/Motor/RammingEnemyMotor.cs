using RAIN.Motion;
using RAIN.Representation;
using RAIN.Serialization;
using System;
using UnityEngine;

[RAINSerializableClass]
public class RammingEnemyMotor : UnityNavMeshMotor {

	private RammingEntity rammingEntity;

	public override void Start() {
		base.Start();

		rammingEntity = AI.Body.GetComponent<RammingEntity>();
	}

	public override bool Move() {
		// Set the area costs
		if(rammingEntity.IsRamming) {
			// Set all costs to ocean cost
		} else {
			// Set the costs according to target
		}

		return base.Move();
	}
}