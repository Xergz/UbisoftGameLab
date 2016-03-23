using RAIN.Action;
using RAIN.Entities.Aspects;
using RAIN.Representation;
using System;
using System.Collections.Generic;
using UnityEngine;

[RAINAction]
public class ParseDetect : RAINAction {

    public Expression aspectsList = new Expression();
    public Expression detectedForm = new Expression();

    private float minimumDistance;
    private float currentDistance;
    private int index;

    public override ActionResult Execute(RAIN.Core.AI ai) {

        if(!aspectsList.IsVariable)
            throw new Exception("The parse detect node requires a valid aspects list variable");

        if (!detectedForm.IsVariable)
            throw new Exception("The parse detect node requires a valid detected form variable");

        // Assigned from a detect node, using the Aspect Variable field, and using All Matches option
        IList<RAINAspect> tAspects = ai.WorkingMemory.GetItem<IList<RAINAspect>>(aspectsList.VariableName);

        minimumDistance = 10000000000;

        for (int i = 0; i < tAspects.Count; i++) {
            currentDistance = Vector3.Distance(ai.Body.transform.position, tAspects[i].Entity.Form.transform.position);
            if (currentDistance < minimumDistance) {
                minimumDistance = currentDistance;
                index = i;
            }
        }

        if(tAspects.Count > 0)
            ai.WorkingMemory.SetItem<GameObject>(detectedForm.VariableName, tAspects[index].Entity.Form);
        else
            ai.WorkingMemory.SetItem<GameObject>(detectedForm.VariableName, null);

        return ActionResult.SUCCESS;
    }
}