using UnityEngine;
using RAIN.Action;
using RAIN.Core;

[RAINAction("Die")]
public class Die : RAINAction {

    public override ActionResult Execute(AI ai) {

        MonoBehaviour.Destroy(ai.Body);

        return ActionResult.SUCCESS;
    }
}