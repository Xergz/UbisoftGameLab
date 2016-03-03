using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Serialization;

[RAINAction("Set End Ram Position")]
public class SetEndRamPosition : RAINAction {
	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The remaining distance to travel")]
	public Expression remainingDistanceVariable = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that the result will be assigned to")]
	public Expression moveTargetVariable = new Expression();

	// The default remaining distance to use when the remaining distance variable is invalid
	private float defaultRemainingDistance = 3f;

	public override ActionResult Execute(AI ai) {
		if(!moveTargetVariable.IsVariable) {
			throw new Exception("The End Ram node requires a valid moveTargetVariable");
		}

		float distance;
		if(!remainingDistanceVariable.IsVariable) {
			distance = defaultRemainingDistance;
		} else {
			distance = ai.WorkingMemory.GetItem<float>(remainingDistanceVariable.VariableName);
		}

		ai.WorkingMemory.SetItem(moveTargetVariable.VariableName, ai.Kinematic.Position + (Vector3.Normalize(ai.Kinematic.Forward) * distance));

		return ActionResult.SUCCESS;
	}
}