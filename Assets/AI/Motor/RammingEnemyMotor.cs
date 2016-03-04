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


	protected override void SetAreaCosts(Vector3 position, Vector3 target) {
		if(rammingEntity.IsRamming) {
			// Set all costs to ocean cost
			StreamController.SetAreaCosts(StreamController.OceanAreaCost);
		} else {
			// Set the costs according to target
			base.SetAreaCosts(position, target);
		}
	}
}