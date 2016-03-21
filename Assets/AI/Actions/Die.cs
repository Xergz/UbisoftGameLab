using UnityEngine;
using RAIN.Action;
using RAIN.Core;

[RAINAction("Die")]
public class Die : RAINAction {

    public override ActionResult Execute(AI ai) {

		ChasingEntity chasingEntity = ai.Body.GetComponent<ChasingEntity>();

		if(chasingEntity != null) {
			chasingEntity.SetDying();
		} else {
			MonoBehaviour.Destroy(ai.Body);
		}

        return ActionResult.SUCCESS;
    }
}