using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    /*classe Power qui change de direction, override la fonction activatePower pour que ca change la direction de la liste qui lui ait donner
      ce diferencie grace au enumPower
    */
public class PowerChangeDirection : Power {

	// Use this for initialization
	void Start () {
        powerType = enumPower.ChangeDirection;

    }
	
	// Update is called once per frame
	void Update () {

        cooldownTimer();
    }
   
    //fonction qui fais le pouvoir selon la liste de stream qui lui ait envoyer
    public override void activatePower(List<Stream> liste)
    {
        liste.ForEach((stream) =>
        {
            stream.SwitchDirection();
        });

        }

}

