using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Navigation;
using RAIN.Serialization;

[RAINAction("Choose Search Position")]
public class ChooseSearchPosition : RAINAction {

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The radius around the last position of the entity to search in")]
	public Expression searchRadius = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The minimum distance for a movement")]
	public Expression minimumMovingDistance = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that the result will be assigned to")]
	public Expression moveTargetVariable = new Expression();

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The name of the variable that contains the position to search around")]
	public Expression searchPositionVariable = new Expression();

	// The default search radius to use when the search radius is invalid
	private float defaultSearchRadius = 10f;

	public override ActionResult Execute(AI ai) {
		if(!moveTargetVariable.IsVariable) {
			throw new Exception("The Choose Search Position node requires a valid moveTargetVariable");
		}

		if(!searchPositionVariable.IsVariable) {
			throw new Exception("The Choose Search Position node requires a valid searchPositionVariable");
		}

		float distance = 0f;
		if(searchRadius.IsValid && minimumMovingDistance.IsValid) {
			distance = UnityEngine.Random.Range(minimumMovingDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory), searchRadius.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory));
		} else {
			distance = defaultSearchRadius;
		}

		Vector3 movement = Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f))) * distance;

		Vector3 destination = ai.WorkingMemory.GetItem<Vector3>(searchPositionVariable.VariableName) + movement;

		ai.WorkingMemory.SetItem(moveTargetVariable.VariableName, destination);

		return ActionResult.SUCCESS;
	}
}