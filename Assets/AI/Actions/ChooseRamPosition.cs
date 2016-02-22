using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Navigation;
using RAIN.Serialization;

[RAINAction("Choose Ram Position")]
public class ChooseRamPosition : RAINAction {
	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The distance to travel when ramming")]
	public Expression rammingDistance = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that the result will be assigned to")]
	public Expression moveTargetVariable = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that contains the position to ram toward")]
	public Expression ramPositionVariable = new Expression();

	// The default ramming distance to use when the ramming distance is invalid
	private float defaultRammingDistance = 10f;

	public override ActionResult Execute(AI ai) {
		if(!moveTargetVariable.IsVariable) {
			throw new Exception("The Choose Ram Position node requires a valid moveTargetVariable");
		}

		if(!ramPositionVariable.IsVariable) {
			throw new Exception("The Choose Ram Position node requires a valid ramPositionVariable");
		}

		float distance;
		if(rammingDistance.IsValid) {
			distance = rammingDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
		} else {
			distance = defaultRammingDistance;
		}

		Vector3 movement = Vector3.Normalize(ai.WorkingMemory.GetItem<Vector3>(ramPositionVariable.VariableName) - ai.Kinematic.Position) * distance;

		Vector3 destination = ai.Kinematic.Position + movement;

		ai.WorkingMemory.SetItem(moveTargetVariable.VariableName, destination);

		return ActionResult.SUCCESS;
	}
}