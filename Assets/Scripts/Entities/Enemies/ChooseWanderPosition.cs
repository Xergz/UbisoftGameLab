using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Navigation;

[RAINAction("Choose Wander Position")]
public class ChooseWanderPosition : RAINAction {

	[Tooltip("The maximum range for the wandering")]
	public Expression MaximumRange = new Expression();

	[Tooltip("The minimum range for the wandering")]
	public Expression MinimumRange = new Expression();

	[Tooltip("WanderTargetVariable is the name of the variable that the result will be assigned to")]
	public Expression WanderTargetVariable = new Expression();

	// The default wander distance to use when the WanderDistance is invalid
	private float defaultWanderDistance = 5f;

	public override ActionResult Execute(AI ai) {
		if(!WanderTargetVariable.IsVariable) {
			throw new Exception("The Choose Wander Position node requires a valid Wander Target Variable");
		}

		float wanderDistance = 0f;
		if(MaximumRange.IsValid && MinimumRange.IsValid) {
			wanderDistance = UnityEngine.Random.Range(MinimumRange.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory), MaximumRange.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory));
		}

		if(wanderDistance <= 0f) {
			wanderDistance = defaultWanderDistance;
		}

		Vector3 direction = Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)));
		direction *= wanderDistance;

		Vector3 destination = ai.Kinematic.Position + direction;

		ai.WorkingMemory.SetItem(WanderTargetVariable.VariableName, destination);

		return ActionResult.SUCCESS;
	}
}