using UnityEngine;
using System.Collections.Generic;

public class SpawnNode : MonoBehaviour {
	[Tooltip("Specify a specific encounter, if left to null, will get a random one from encounter manager")]
	public Encounter SpecialEncounterSpawn = null;
	[Tooltip("Need to be given the encounter manage for each node")]
	public EncounterManager EncounterManager = null;
	[Tooltip("Need to be given the waves manage for each node")]
	public WaveController WaveManager = null;
    public bool enableRespawn = false;
    
	[SerializeField]
	private bool spawned = false;

	private bool spawnedOnce = false;
    [SerializeField]
    private float delayBeforeRespawn = 2f;
    private float lastTimeSpawned = 0.0f;


	private void OnDrawGizmos() {
		if(SpecialEncounterSpawn != null) {
			SpecialEncounterSpawn.entities.ForEach((entity) => {
				Vector3 position = transform.TransformPoint(new Vector3(entity.spawnPoint.x, entity.spawnPoint.y + 1, entity.spawnPoint.z));
				if(entity.entity is StunEntity) {
					Gizmos.color = Color.cyan;
					Gizmos.DrawCube(position, new Vector3(4, 4, 4));
				} else if(entity.entity is ChasingEntity) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere(position, 1);
				} else {
					Gizmos.color = Color.red;
					Gizmos.DrawCube(position, new Vector3(2, 2, 2));
				}
				Gizmos.DrawLine(transform.position, position);
			});
		}
	}

	private void OnTriggerStay(Collider other) {
		if(other.gameObject.tag == "Player" && (!spawned || (enableRespawn && Time.time - lastTimeSpawned > delayBeforeRespawn))) {
                spawned = true;
            lastTimeSpawned = Time.time;
            SpawnEncounter();
		}
	}

	private void SpawnEncounter() {
		if(SpecialEncounterSpawn != null) {
			//So ugly...
			SpecialEncounterSpawn.SpawnEntities(transform, transform.localPosition, EncounterManager, WaveManager, !spawnedOnce);
		} else {
			EncounterManager.GetEncounter().SpawnEntities(transform, transform.localPosition, EncounterManager, WaveManager, !spawnedOnce);
		}

		spawnedOnce = true;
	}
}
