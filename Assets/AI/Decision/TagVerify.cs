using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINDecision("Tag Verify")]
public class TagVerify : RAINDecision
{
	public Expression gameObjectForm = new Expression();
	public Expression tag = new Expression();

    private int _lastRunning = 0;
	private string tagToVerify;
	private GameObject gameObjectToverify;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

        _lastRunning = 0;
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        ActionResult tResult = ActionResult.SUCCESS;

		if (tag.IsValid)
			tagToVerify = tag.Evaluate<string>(ai.DeltaTime, ai.WorkingMemory);

		if (!gameObjectForm.IsVariable)
			throw new Exception("The Tag Verify decision requires a valid game object variable");

		gameObjectToverify = ai.WorkingMemory.GetItem<GameObject>(gameObjectForm.VariableName);

		if(gameObjectToverify == null || tagToVerify != gameObjectToverify.tag)
			return ActionResult.FAILURE;

        for (; _lastRunning < _children.Count; _lastRunning++)
        {
            tResult = _children[_lastRunning].Run(ai);
            if (tResult != ActionResult.SUCCESS)
                break;
        }

        return tResult;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}