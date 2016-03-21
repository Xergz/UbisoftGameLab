using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINDecision]
public class Timed : RAINDecision
{
    public Expression time = new Expression();

    private int _lastRunning = 0;
    private float _time = 0;
    private float initialTime = -1f;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

        _lastRunning = 0;
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        ActionResult tResult = ActionResult.SUCCESS;

        if (time.IsValid)
            _time = time.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);

        if (initialTime < 0)
            initialTime = Time.time;

        if (Time.time - initialTime >= _time) {
            initialTime = -1f;
            return ActionResult.FAILURE;
        }
            

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