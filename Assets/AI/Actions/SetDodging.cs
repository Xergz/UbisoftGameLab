using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Serialization;

[RAINAction]
public class SetDodging : RAINAction {

    [RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "Whether the entity is ramming or not")]
    public Expression isDodging = new Expression();

    public override ActionResult Execute(AI ai) {
        if (isDodging.IsValid) {
            ai.Body.GetComponent<ChasingEntity>().IsDodging = isDodging.Evaluate<bool>(ai.DeltaTime, ai.WorkingMemory);
            return ActionResult.SUCCESS;
        } else {
            return ActionResult.FAILURE;
        }
    }
}