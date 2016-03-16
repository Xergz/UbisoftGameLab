﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class Encounter : MonoBehaviour
{

    [SerializeField]
    List<SpawnEntity> entities = null;

    NavMeshHit hit;

    public void SpawnEntities(EncounterManager manager, WaveController waveController) 
    {

        SpawnEntities(Vector3.zero, manager, waveController);
    }

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

    public void SpawnEntities(Vector3 localPosition, EncounterManager manager, WaveController waveController)
    {
        foreach (SpawnEntity entity in entities)
        {
            var spawnAt = entity.spawnPoint;
            var rotation = entity.spawnPoint;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(localPosition, out hit, 10.0f, NavMesh.AllAreas))
            {
                var spawned = Instantiate(entity.entity, hit.position + entity.spawnPoint, Quaternion.identity) as Entity;
                var wave = spawned.GetComponent<FloatOnWave>();
                
                wave.waves = waveController;
                manager.addEntity(spawned);
                spawned.gameObject.SetActive(true);
                Debug.Log("Spawn at " + (hit.position + entity.spawnPoint).x + "," + (hit.position + entity.spawnPoint).y +","+ (hit.position + entity.spawnPoint).z);
            }           
        }
    }
}