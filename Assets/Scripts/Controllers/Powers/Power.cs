﻿using UnityEngine;

public abstract class Power : MonoBehaviour {

	public bool IsReady { get { return ready; } }

	public EnumPower PowerType { get; protected set; }

	[Tooltip("The cooldown time for this power")]
	[SerializeField]
	protected float cooldownTime = 3.0F;
	[Tooltip("This should not be changed and is for debugging purposes only")]
	[SerializeField]
	protected float elapsedTime = 0; // The elapsed time since the power has been used

	protected bool ready = true;

	/// <summary>
	/// Activates the power
	/// </summary>
	public void Activate() {
		if(ready) {
			ExecuteAction();
			ready = false;
		}
	}

	/// <summary>
	/// Get the time left to the cooldown
	/// </summary>
	/// <returns></returns>
	public float GetTimeLeftToCooldown() {
		return cooldownTime - elapsedTime;
	}


	/// <summary>
	/// Executes the power
	/// </summary>
	protected abstract void ExecuteAction();


	/// <summary>
	/// Update the elapsed time until it reaches the cooldown time
	/// </summary>
	private void UpdateCooldown() {
		if(!ready) {
			if(elapsedTime < cooldownTime) {
				elapsedTime += Time.deltaTime;
			} else {
				elapsedTime = 0F;
				ready = true;
			}
		}
	}


	private void Start() {
		if(Application.isPlaying) {
			PowerController.RegisterPower(this); // We must wait for when the PowerController will be initialized so we use Start
		}
	}

	private void Update() {
		UpdateCooldown();
	}
}


