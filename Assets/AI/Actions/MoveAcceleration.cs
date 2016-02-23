using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Serialization;
using RAIN.Representation;
using RAIN.BehaviorTrees.Actions;

[RAINAction("Move with Acceleration")]
public class MoveAcceleration : MoveAction {
	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The new acceleration")]
	public Expression acceleration = new Expression();

	public override ActionResult Execute(AI ai) {
		if(acceleration.IsValid) {
			(ai.Motor as UnityNavMeshMotor).Acceleration = acceleration.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
		} else {
			(ai.Motor as UnityNavMeshMotor).Acceleration = (ai.Motor as UnityNavMeshMotor).DefaultAcceleration;
		}

		return base.Execute(ai);
	}
}