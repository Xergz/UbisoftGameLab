using UnityEngine;
using System.Collections;

public class Boost : Power
{
    PlayerController playerController { get; set; }
    private float timeSinceLastBoost = 0.0f;
    private bool powerboost = false;

    // Use this for initialization
    private void Awake()
    {
        PowerType = EnumPower.boost;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void ExecuteAction()
    {

        powerboost = true;
        timeSinceLastBoost = Time.time;
        //playerController.speedMultiplierBoost = 2.5f;
    }

    public override void stopPower()
    {
        if (powerboost)
        {
            powerboost = false;
            timeSinceLastBoost = Time.time;
           // playerController.speedMultiplierBoost = 1f;
        }
    }
    public override void checkPowerTIme()
    {
        if (Time.time - timeSinceLastBoost > 1.5f && powerboost)
        {
            stopPower();
        }
    }
}