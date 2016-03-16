using UnityEngine;
using System.Collections;

public class SpawnNode : MonoBehaviour {
    [Tooltip("Specify a specific encounter, if left to null, will get a random one from encounter manager")]
    public Encounter SpecialEncounterSpawn =  null;
    [Tooltip("Need to be given the encounter manage for each node")]
    public EncounterManager EncounterManager =  null;
    [Tooltip("Need to be given the waves manage for each node")]
    public WaveController WaveManager = null;
    private bool spawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player enters spawn node");
            spawnEncounter();
        }
            
    }

    private void spawnEncounter()
    {
        if (!spawned)
        {
            spawned = true;
            if (SpecialEncounterSpawn != null)
            {
                //So ugly...
                SpecialEncounterSpawn.SpawnEntities(transform.localPosition, EncounterManager, WaveManager);
            }
            else
            {
                EncounterManager.GetEncounter().SpawnEntities(transform.position,EncounterManager, WaveManager);
            }
        }

    }
}
