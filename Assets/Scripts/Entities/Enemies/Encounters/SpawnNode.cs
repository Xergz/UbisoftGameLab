using UnityEngine;
using System.Collections;

public class SpawnNode : MonoBehaviour {
	[Tooltip("Specify a specific encounter, if left to null, will get a random one from encounter manager")]
	public Encounter SpecialEncounterSpawn = null;
	[Tooltip("Need to be given the encounter manage for each node")]
	public EncounterManager EncounterManager = null;
	[Tooltip("Need to be given the waves manage for each node")]
	public WaveController WaveManager = null;


	[SerializeField]
	private bool spawned = false;


	private void OnTriggerStay(Collider other) {
		if(other.gameObject.tag == "Player" && !spawned) {
			spawned = true;
			SpawnEncounter();
		}
	}

	private void SpawnEncounter() {
		if(SpecialEncounterSpawn != null) {
			//So ugly...
			SpecialEncounterSpawn.SpawnEntities(transform, transform.localPosition, EncounterManager, WaveManager);
		} else {
			EncounterManager.GetEncounter().SpawnEntities(transform, transform.localPosition, EncounterManager, WaveManager);
		}
	}
}
