﻿using UnityEngine;
using UnityEngine.UI;

public abstract class Power : MonoBehaviour {

	//public bool IsReady { get { return ready; } }

	public EnumPower PowerType { get; protected set; }
	/*
    public Image CooldownBar;

    [Tooltip("The cooldown time for this power")]
	[SerializeField]
	protected float cooldownTime = 3.0F;
	[Tooltip("This should not be changed and is for debugging purposes only")]
	[SerializeField]
	protected float elapsedTime = 0; // The elapsed time since the power has been used
	protected float cooldownMultiplier = 1F; // The multiplier for the cooldown time

	protected bool ready = true;

	protected Color cooldownColor = Color.white;*/

	/// <summary>
	/// Activates the power
	/// </summary>
	public void Activate() {
		//if(ready) {
		ExecuteAction();
		//ready = false;
		//}
	}

	/// <summary>
	/// Get the time left to the cooldown
	/// </summary>
	/// <returns></returns>
	/*public float GetTimeLeftToCooldown() {
		return (cooldownTime * cooldownMultiplier) - elapsedTime;
	}

	public void SetCooldownMultiplier(float multiplier) {
		cooldownMultiplier = multiplier;
	}*/


	/// <summary>
	/// Executes the power
	/// </summary>
	protected abstract void ExecuteAction();

    public virtual void stopPower()
    {
        //to be redeclared in powers that need a stop
    }
    public virtual void checkPowerTIme()
    {
        //to be redeclared in powers that need a timer
    }

    /// <summary>
    /// Update the elapsed time until it reaches the cooldown time
    /// </summary>
    /*private void UpdateCooldown() {
		if(!ready) {
			float timeLeft = GetTimeLeftToCooldown();
			if(timeLeft > 0.01) {
				elapsedTime += Time.deltaTime;
				CooldownBar.color = cooldownColor;
				CooldownBar.fillAmount = (timeLeft / (cooldownTime * cooldownMultiplier));			
            } else {
				elapsedTime = 0F;
				ready = true;
                CooldownBar.fillAmount = 0;
				CooldownBar.color = Color.white;
            }
        }
	}*/

    private void Start() {
		if(Application.isPlaying) {
			PowerController.RegisterPower(this); // We must wait for when the PowerController will be initialized so we use Start
												 //CooldownBar.fillAmount = 0;
												 //CooldownBar.color = Color.white;
		}
	}

	/*private void Update() {
			UpdateCooldown();
		}
	}*/
}



