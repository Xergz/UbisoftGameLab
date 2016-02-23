using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Navigation;
using RAIN.Serialization;

[RAINAction("Choose Wander Position")]
public class ChooseWanderPosition : RAINAction {

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The maximum range for the wandering")]
	public Expression maximumRange = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The minimum range for the wandering")]
	public Expression minimumRange = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that the result will be assigned to")]
	public Expression wanderTargetVariable = new Expression();

	// The default wander distance to use when the maximum or minimum range is invalid
	private float defaultWanderDistance = 5f;

	public override ActionResult Execute(AI ai) {
		if(!wanderTargetVariable.IsVariable) {
			throw new Exception("The Choose Wander Position node requires a valid Wander Target Variable");
		}

		float wanderDistance = 0f;
		if(maximumRange.IsValid && minimumRange.IsValid) {
			wanderDistance = UnityEngine.Random.Range(minimumRange.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory), maximumRange.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory));
		} else {
			wanderDistance = defaultWanderDistance;
		}

		Vector3 movement = Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f))) * wanderDistance;

		Vector3 destination = ai.Kinematic.Position + movement;

		ai.WorkingMemory.SetItem(wanderTargetVariable.VariableName, destination);

		return ActionResult.SUCCESS;
	}
}