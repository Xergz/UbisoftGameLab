using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Encounter : ScriptableObject {

	public List<SpawnEntity> entities = null;

#if UNITY_EDITOR
	[MenuItem("Assets/Create/Encounter")]
	public static void CreateMyAsset() {
		Encounter encounter = CreateInstance<Encounter>();

		AssetDatabase.CreateAsset(encounter, "Assets/Prefabs/Entities/Enemies/Encounters/Encounter.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = encounter;
	}
#endif

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
				NavMeshHit hit;
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