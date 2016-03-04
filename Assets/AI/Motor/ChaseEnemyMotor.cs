using RAIN.Motion;
using RAIN.Representation;
using RAIN.Serialization;
using System;
using UnityEngine;

[RAINSerializableClass]
public class ChaseEnemyMotor : UnityNavMeshMotor {

    private ChasingEntity chasingEntity;

    public override void Start() {
        base.Start();

        chasingEntity = AI.Body.GetComponent<ChasingEntity>();
    }


    protected override void SetAreaCosts(Vector3 position, Vector3 target) {
        if (chasingEntity.IsDodging) {
            // Set all costs to ocean cost
            StreamController.SetAreaCosts(StreamController.OceanAreaCost);
        } else {
            // Set the costs according to target
            base.SetAreaCosts(position, target);
        }
    }
}