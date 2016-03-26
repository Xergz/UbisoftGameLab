using UnityEngine;
using System.Collections.Generic;
using System;

public class Encounter : MonoBehaviour {

	[SerializeField]
	List<SpawnEntity> entities = null;

	NavMeshHit hit;

	[System.Serializable]
	public class SpawnEntity {
		public Entity entity;
		public Vector3 spawnPoint = new Vector3();

		public SpawnEntity(Entity ent, Vector3 point) {
			entity = ent;
			spawnPoint = point;
		}
	}

	public void SpawnEntities(Transform root, Vector3 localPosition, EncounterManager manager, WaveController waveController, bool spawnHands) {
		foreach(SpawnEntity entity in entities) {
			if(!entity.entity.CompareTag("Hand") || spawnHands) {
				Vector3 spawnPosition = localPosition + entity.spawnPoint;
				if(NavMesh.SamplePosition(spawnPosition, out hit, 10.0f, NavMesh.AllAreas)) {
					var spawned = Instantiate(entity.entity, hit.position, Quaternion.identity) as Entity;
					spawned.gameObject.SetActive(false);
					var wave = spawned.GetComponent<FloatOnWave>();

					wave.waves = waveController;
					manager.addEntity(spawned);
					spawned.transform.parent = root;
					spawned.gameObject.SetActive(true);
					Debug.Log("Spawned entity at location X(" + hit.position.x + "), Y(" + hit.position.y + "), Z(" + hit.position.z + ")");
				} else {
					Debug.LogError("Could not spawn the entity at location: X(" + spawnPosition.x + "), Y(" + spawnPosition.y + "), Z(" + spawnPosition.z + ")");
				}
			}
		}
	}
}