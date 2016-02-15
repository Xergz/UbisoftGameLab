using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerController : MonoBehaviour {


    List<Power> powers = new List<Power>();
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool isPowerReady(enumPower power)
    {
        for(int i=0; i < powers.Count; i++)
        {
            if (powers[i].getPowerType() == power)
            {
                return powers[i].ispowerReady();
            }
        }
        return false;
    }
    public bool activatePower(enumPower power, List<Stream> listStream)
    {
        for(int i=0; i < powers.Count; i++)
        {
            
            if (powers[i].getPowerType() == power)
            {
                if (powers[i].ispowerReady())
                {
                    powers[i].activatePower(listStream);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
}
