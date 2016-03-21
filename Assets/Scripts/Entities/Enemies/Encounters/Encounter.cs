using UnityEngine;
using System.Collections.Generic;
using System;

public class Encounter : MonoBehaviour
{

    [SerializeField]
    List<SpawnEntity> entities = null;

    NavMeshHit hit;

    [System.Serializable]
    public class SpawnEntity
    {
        public Entity entity;
        public Vector3 spawnPoint =  new Vector3();

        public SpawnEntity(Entity ent, Vector3 point)
        {
            entity = ent;
            spawnPoint = point;
        }
    }

    public void SpawnEntities(Transform root, Vector3 localPosition, EncounterManager manager, WaveController waveController)
    {
        foreach (SpawnEntity entity in entities)
        {
			if (NavMesh.SamplePosition(root.TransformPoint(localPosition + entity.spawnPoint), out hit, 10.0f, NavMesh.AllAreas))
            {
                var spawned = Instantiate(entity.entity, hit.position, Quaternion.identity) as Entity;
                var wave = spawned.GetComponent<FloatOnWave>();
                
                wave.waves = waveController;
                manager.addEntity(spawned);
				spawned.transform.parent = root;
                spawned.gameObject.SetActive(true);
                Debug.Log("Spawn at " + hit.position.x + "," + hit.position.y +","+ hit.position.z);
            }           
        }
    }
}