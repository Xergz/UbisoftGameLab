﻿using UnityEngine;
using System;
using System.Collections;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Entities;
using RAIN.Entities.Aspects;

[RAINAction("Attack")]
public class Attack : RAINAction {

    public Expression targetVariable = new Expression();

    private Entity targetEntity;

    public override ActionResult Execute(AI ai) {

        if (!targetVariable.IsVariable)
            throw new Exception("The Attack node requires a valid Target Variable");

        if (targetVariable.IsValid)
            targetEntity = targetVariable.Evaluate<Entity>(ai.DeltaTime, ai.WorkingMemory);

        if (targetEntity == null)
            return ActionResult.FAILURE;

        targetEntity.ReceiveHit();

        return ActionResult.SUCCESS;
    }
}