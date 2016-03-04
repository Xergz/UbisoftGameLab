using UnityEngine;
using System;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Perception.Sensors;
using RAIN.Entities.Aspects;

[RAINAction("Detect Tag (Hearing)")]
public class DetectTagHearing : RAINAction {

    public Expression gameObjectsList = new Expression();
    public Expression tag = new Expression();

    private string tagToVerify;

    public override ActionResult Execute(AI ai) {

		if (!gameObjectsList.IsVariable)
            throw new Exception("The Detect Tag (Hearing) node requires a valid attacker variable");

        if (tag.IsValid)
            tagToVerify = tag.Evaluate<string>(ai.DeltaTime, ai.WorkingMemory);

        RAINSensor tSensor = ai.Senses.GetSensor("Hearing");

        if (tSensor != null) {
            tSensor.Sense("entityAudio", RAINSensor.MatchType.ALL);
            IList<RAINAspect> tAspects = tSensor.Matches;

            List<GameObject> entities = new List<GameObject>();

            for (int i = 0; i < tAspects.Count; ++i) {
                if (tAspects[i].Entity.Form.tag == tagToVerify) {
                    entities.Add(tAspects[i].Entity.Form);
                }
            }

            if(entities.Count > 0) {
				ai.WorkingMemory.SetItem<List<GameObject>>(gameObjectsList.VariableName, entities);
                return ActionResult.SUCCESS;
            }
        }

		return ActionResult.NONE;
    }
}
