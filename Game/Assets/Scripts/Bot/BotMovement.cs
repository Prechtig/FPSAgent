using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	private Transform[] waypoints;
	private int currentWaypoint;

	public float moveSpeed;
	// Use this for initialization
	void Start () {
		GameObject[] ways = GameObject.FindGameObjectsWithTag ("Waypoint");

		waypoints = new Transform[ways.Length];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = ways [i].transform;
		}

		currentWaypoint = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float step =  moveSpeed * Time.deltaTime;
		if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5) {
			// At waypoint so stop moving
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
