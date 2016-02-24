using UnityEngine;
using System;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Perception.Sensors;
using RAIN.Entities.Aspects;
using RAIN.Motion;

[RAINAction("Dodge")]
public class Dodge : RAINAction {

    public Expression attackerForm = new Expression();
    public Expression dodgeDistance = new Expression();
    public Expression safePosition = new Expression();

    private GameObject attackerGameObject;

    private Transform attackerTransform;

    private Vector3 currentPosition;

    private float dodgeLength;

    public override ActionResult Execute(AI ai) {

        if (!safePosition.IsVariable)
            throw new Exception("The Dodge node requires a valid safe position variable");

        if (!attackerForm.IsVariable)
            throw new Exception("The Dodge node requires a valid attacker variable");

		attackerGameObject = ai.WorkingMemory.GetItem<GameObject>(attackerForm.VariableName);

        if (dodgeDistance.IsValid)
            dodgeLength = dodgeDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);

        currentPosition = new Vector3(ai.Body.transform.position.x, 0, ai.Body.transform.position.z);

		attackerTransform = attackerGameObject.transform;
        attackerTransform.position = new Vector3(attackerTransform.position.x, 0, attackerTransform.position.z);
        attackerTransform.forward = new Vector3(attackerTransform.forward.x, 0, attackerTransform.forward.z);
         
        if(attackerTransform == null) {
            return ActionResult.FAILURE;
        }

        float direction = 1;
        if (Vector3.Dot(Vector3.up, Vector3.Cross(attackerTransform.forward, currentPosition - attackerTransform.position)) < 0) {
            direction = -direction;
        }

        Vector3 distance = attackerTransform.position - currentPosition;

        distance = Vector3.Normalize(new Vector3(-distance.z, 0, distance.x)) * dodgeLength * direction;

        ai.WorkingMemory.SetItem<Vector3>(safePosition.VariableName, distance);
 
        return ActionResult.FAILURE;
    }
}