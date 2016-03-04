using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Serialization;
using System;

[RAINAction("Set Remaining Distance")]
public class SetRemainingDistance : RAINAction {
	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The distance to travel when ramming")]
	public Expression rammingDistance = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable in which to store the remaining distance")]
	public Expression distanceVariable = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable which contains the target position")]
	public Expression targetPositionVariable = new Expression();

	// The default ramming distance to use when the ramming distance is invalid
	private float defaultRammingDistance = 10f;

	public override ActionResult Execute(AI ai) {
		if(!distanceVariable.IsVariable) {
			throw new Exception("The Set Remaining Distance node requires a valid distanceVariable");
		}

		if(!targetPositionVariable.IsVariable) {
			throw new Exception("The Set Remaining Distance node requires a valid targetPositionVariable");
		}

		float distance;
		if(rammingDistance.IsValid) {
			distance = rammingDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
		} else {
			distance = defaultRammingDistance;
		}

		ai.WorkingMemory.SetItem(distanceVariable.VariableName, distance - Vector3.Distance(ai.Kinematic.Position, ai.WorkingMemory.GetItem<Vector3>(targetPositionVariable.VariableName)));

		return ActionResult.SUCCESS;
	}
}