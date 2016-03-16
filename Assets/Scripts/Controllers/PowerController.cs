using UnityEngine;
using System.Collections.Generic;

public class PowerController : MonoBehaviour {
	private static List<Power> powers;


	/// <summary>
	/// Register a power to this power controller
	/// </summary>
	/// <param name="power">The power to register</param>
	public static void RegisterPower(Power power) {
		powers.Add(power);
	}

	/// <summary>
	/// If the power is ready to be used
	/// </summary>
	/// <param name="type">The type of the power</param>
	/// <returns>Whether the power is ready or not</returns>
	public bool IsPowerReady(EnumPower type) {
		foreach(Power power in powers) {
			if(power.PowerType == type) {
				return power.IsReady;
			}
		}
		return false;
	}

	/// <summary>
	/// Get the power to set it up before activation
	/// </summary>
	/// <param name="type">The type of the power</param>
	/// <returns>The power</returns>
	public Power GetPower(EnumPower type) {
		foreach(Power power in powers) {
			if(power.PowerType == type) {
				return power;
			}
		}
		return null;
	}

	/// <summary>
	/// Activates a power. May need setup first (use GetPower for that)
	/// </summary>
	/// <param name="type">The type of the power</param>
	public void ActivatePower(EnumPower type) {
		foreach(Power power in powers) {
			if(power.PowerType == type) {
				power.Activate();
				break;
			}
		}
	}


	private void Awake() {
		powers = new List<Power>();
	}

    public void resetTimer(EnumPower type)
    {
        foreach (Power power in powers)
        {
            if (power.PowerType == type)
            {
                power.resetTimer();
            }
        }
    }
}
