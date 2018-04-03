using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavigationPath {
	public bool looping;
	public float target_speed;
	public Transform[] waypoints;
	[HideInInspector] public int current_target_waypoint = 0;
}
