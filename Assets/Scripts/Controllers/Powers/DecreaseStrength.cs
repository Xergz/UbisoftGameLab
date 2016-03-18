using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecreaseStrength : Power {

    public Stream stream { get; set; } // The streams on which the power should apply
    private float strengthIncreaseSpeed = 1F;
    public float value;

    private void Awake()
    {
        PowerType = EnumPower.DecreaseStrength;
        value = 0f;
    }

    /// <summary>
    /// Switches the direction of the streams in the current stream list. This list should always be set before calling this.
    /// </summary>
    protected override void ExecuteAction()
    {
        stream.DecreaseStrength(value * strengthIncreaseSpeed);

    }
}
