using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using System;

[RAINAction]
public class StillATarget : RAINAction
{
    public Expression attackerForm = new Expression();

    private GameObject attackerObj;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if (!attackerForm.IsVariable)
            throw new Exception("The Still A Target node requires a valid attacker variable");

        attackerObj = ai.WorkingMemory.GetItem<GameObject>(attackerForm.VariableName);

        if (Vector3.Angle(attackerObj.transform.forward, ai.Body.transform.position - attackerObj.transform.position) < 40)
            return ActionResult.RUNNING;

        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}