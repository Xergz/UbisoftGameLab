using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour {

    private static EncounterManager instance;

    [SerializeField]
    Encounter[] possibleEncounters;

    List<Entity> spawnedEntities = new List<Entity>();
	List<Entity> toDestroy = new List<Entity>();
    public float distanceToDespawn = 100f;

    private EncounterManager() { }

    void Awake()
    {
        StartCoroutine("checkToDespawn");
    }

    public void addEntity(Entity ent)
    {
        spawnedEntities.Add(ent);
    }

    public static EncounterManager Instance
    {
        get
        {
            if (instance == null) {
                instance = new EncounterManager();
            }
            return instance;
        }
    }

    public IEnumerator checkToDespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            foreach(Entity ent in spawnedEntities)
            {
                if(ent.Distance > distanceToDespawn)
                {
                    Debug.Log("Despawning entity" + ent.name);
                    toDestroy.Add(ent);
                }
            }

			foreach(Entity ent in toDestroy) {
				spawnedEntities.Remove(ent);
				Destroy(ent.gameObject);
			}

			toDestroy.Clear();
        }
    }

    public Encounter GetEncounter(int specificIndex)
    {
        return possibleEncounters[specificIndex];
    }

    public Encounter GetEncounter()
    {
        return possibleEncounters[Random.Range(0, possibleEncounters.Length-1)];
    }

}
