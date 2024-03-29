﻿using UnityEngine;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

	[Tooltip("The zone index that will be transmited to all children streams")]
	[SerializeField]
	private EnumZone zone;

	public EnumZone ZoneIndex { get { return zone; } }

	// Use this for initialization
	void Start() {
		Stream[] streams = GetComponentsInChildren<Stream>();
		List<int> areasSet = new List<int>();
		foreach(Stream stream in streams) {
			stream.Zone = zone;
			if(!areasSet.Contains(stream.AreaIndex)) {
				areasSet.Add(stream.AreaIndex);
			} else {
				Debug.LogError("Area " + stream.AreaIndex + " exists more than once in zone " + zone);
			}
		}

		LevelWaypoint[] waypoints = GetComponentsInChildren<LevelWaypoint>();
		foreach(LevelWaypoint waypoint in waypoints) {
			waypoint.Zone = zone;
		}
	}
}
