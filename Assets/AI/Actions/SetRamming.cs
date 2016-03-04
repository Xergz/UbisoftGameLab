using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Serialization;

[RAINAction]
public class SetRamming : RAINAction {

	[RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "Whether the entity is ramming or not")]
	public Expression isRamming = new Expression();

	public override ActionResult Execute(AI ai) {
		if(isRamming.IsValid) {
			ai.Body.GetComponent<RammingEntity>().IsRamming = isRamming.Evaluate<bool>(ai.DeltaTime, ai.WorkingMemory);
			return ActionResult.SUCCESS;
		} else {
			return ActionResult.FAILURE;
		}
	}
}