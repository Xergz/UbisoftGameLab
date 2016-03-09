using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINAction("Choose Attacker")]
public class ChooseAttacker : RAINAction
{
	public Expression attackersList = new Expression();
	public Expression chosenAttacker = new Expression();

	private List<GameObject> attackersGameObject;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		if (!attackersList.IsVariable)
			throw new Exception("The Choose Attacker node requires a valid attackers list variable");

		if (!chosenAttacker.IsVariable)
			throw new Exception("The Choose Attacker node requires a valid chosen attacker variable");

		attackersGameObject = ai.WorkingMemory.GetItem<List<GameObject>>(attackersList.VariableName);

		foreach(GameObject element in attackersGameObject) {
			if(Vector3.Angle(element.transform.forward, ai.Body.transform.position - element.transform.position) < 20) {
				ai.WorkingMemory.SetItem<GameObject>(chosenAttacker.VariableName, element);
				return ActionResult.SUCCESS;
			}
		}

        return ActionResult.NONE;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}