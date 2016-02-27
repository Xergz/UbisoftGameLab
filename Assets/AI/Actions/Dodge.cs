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

        float direction = 1;
		if (Vector3.Dot(Vector3.up, Vector3.Cross(ai.Body.transform.position - attackerGameObject.transform.position, attackerGameObject.transform.forward)) < 0) {
            direction = -direction;
        }
			
		Vector3 distance = Vector3.Normalize(Vector3.Cross(ai.Body.transform.position - attackerGameObject.transform.position, attackerGameObject.transform.up)) * direction * dodgeLength + ai.Body.transform.position;

        ai.WorkingMemory.SetItem<Vector3>(safePosition.VariableName, distance);
 
		return ActionResult.SUCCESS;
    }
}