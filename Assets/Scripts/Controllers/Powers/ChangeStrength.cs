using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Changestrength : Power
{

    public List<Stream> streams { get; set; } // The streams on which the power should apply

    private void Awake()
    {
        streams = new List<Stream>();
        PowerType = EnumPower.SwitchDirection;
    }

    /// <summary>
    /// Switches the direction of the streams in the current stream list. This list should always be set before calling this.
    /// </summary>
    protected override void ExecuteAction()
    {

    }
}
