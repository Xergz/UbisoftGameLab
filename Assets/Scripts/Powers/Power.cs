using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//classe power qui est abstracte pour tout les pouvoir


public abstract class Power : MonoBehaviour {



    public float COOLDOWN_POWER=3.0f;//3.0 parce que c'est ce que le doncument de debut de projet a dit mai selle est public en se moment pour pouvoir la changer
    protected float cooldownTime = 0;//valeur qui sajoute pour savoir combien de temps sajoute
    protected enumPower powerType;


    // Use this for initialization
    void Start (){


    }
	
	// Update is called once per frame
	void Update () {
        
	}

    //update timer

    protected void cooldownTimer()
    {
        if (cooldownTime < 3.0f)
        {
            cooldownTime = cooldownTime + Time.deltaTime;
        }
    }

    //activate power qui va etre differend pour toute les pouvoir
    public abstract void activatePower(List<Stream> liste);
    
    //retourne vrai si le cooldown est pret
    public bool ispowerReady()
    {
        if (cooldownTime >= COOLDOWN_POWER)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //retourne le type du pourovir
    public enumPower getPowerType()
        {
            return powerType;
        }
    //setter du pouvoir
    public void setPowerType(enumPower value)
       {
            powerType = value;
        }
    
    //retourne le temps restant au cooldown

    public float getCooldownTime()
        {
            if (this.ispowerReady())
            {
                return 0.0f;
            }
            else
            {
                return COOLDOWN_POWER - cooldownTime;
            }
        }
    
    }

   

