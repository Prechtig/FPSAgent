using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour
{

    //public Transform[] waypoints;
    //private int currentWaypoint;
    GameObject waypoint;

    public float X
    {
        get; set;
    }

    public float yOffset
    {
        get; set;
    }

    public float zOffset
    {
        get; set;
    }

    public float moveSpeed = 2;
    // Use this for initialization
    void Start()
    {
        //currentWaypoint = 0;
        waypoint = new GameObject("Empty");
        waypoint.transform.position = GenerateSpawnPoint();
    }

    public Vector3 GenerateSpawnPoint()
    {
        float rX = Random.Range(-(X / 2) + 1, X / 2 - 1);
        float y = Random.Range(0, 15);
        y += yOffset + 0.75f;
        return new Vector3(rX, y, zOffset);
    }

    // Update is called once per frame
    void Update()
    {
        float step = moveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, waypoint.transform.position) < 0.5)
        {
            // At waypoint so stops moving
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            waypoint.transform.position = GenerateSpawnPoint();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoint.transform.position, step);
        }


        /*
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
        */
    }

    public void OnDestroy()
    {
        Destroy(waypoint);
    }
}
