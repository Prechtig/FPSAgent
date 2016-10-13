﻿using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	public Transform[] waypoints;
	private int currentWaypoint;

	public float moveSpeed = 2;
	// Use this for initialization
	void Start () {
		currentWaypoint = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float step =  moveSpeed * Time.deltaTime;
		if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5) {
			// At waypoint so stops moving
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
			if (currentWaypoint == waypoints.Length - 1) {
				currentWaypoint = 0;
				transform.LookAt(waypoints[currentWaypoint].transform);
			} else {
				currentWaypoint++;
				transform.LookAt(waypoints[currentWaypoint].transform);
			}
		} else {
			transform.position = Vector3.MoveTowards (transform.position, waypoints [currentWaypoint].position, step);
		}
	}
}
