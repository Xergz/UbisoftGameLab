using UnityEngine;
using System;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINAction]
public class Stun : RAINAction
{
    public Expression victimForm = new Expression();

    public Entity victim;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if(!victimForm.IsVariable)
            throw new Exception("The Stun node requires a valid safe victim variable");

        victim = ai.WorkingMemory.GetItem<GameObject>(victimForm.VariableName).GetComponent<Entity>();

        if(victim != null) {
            victim.ReceiveStun();
            return ActionResult.SUCCESS;
        }

        return ActionResult.FAILURE;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}