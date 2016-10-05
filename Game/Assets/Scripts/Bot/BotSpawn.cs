using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotSpawn : MonoBehaviour
{
	public GameObject bot;
	private IList<GameObject> bots;
	public Transform[] spawnPoints;
	private float spawnTime;
	private static int botsToSpawn = 1;
	private int BotsKilled = 0;

	//public static int iteration = 1;

	public float X;
	public float Z;

	private GameObject spawnObject;

	public BotSpawn (Transform[] spawnPoints){
		this.spawnPoints = spawnPoints;
	}

	// Use this for initialization
	void Start ()
	{
		/*
		bot = Resources.Load ("BotPreFab") as GameObject;
		bot.GetComponent<BotMovement> ().waypoints = spawnPoints;
		//bm.waypoints = spawnPoints;
		//player = (GameObject)Instantiate(a, spawnPoints.position, spawnPoints.rotation);
		InvokeRepeating ("Spawn", 0, spawnTime);
		*/
	}

	public void StartSpawning(){
		bots = new List<GameObject>();
		bot = Resources.Load ("BotPrefab") as GameObject;
		spawnTime = 3f;
		//bot.GetComponent<BotMovement> ().waypoints = spawnPoints;
		//bm.waypoints = spawnPoints;
		//player = (GameObject)Instantiate(a, spawnPoints.position, spawnPoints.rotation);
		InvokeRepeating ("Spawn", 0, spawnTime);
	}

	void Spawn ()
	{
		if(bots.Count < botsToSpawn){
			Vector3 spawnPosition = GenerateSpawnPoint ();
			GameObject b = Instantiate (bot, spawnPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
			//b.GetComponent<BotMovement> ().waypoints = spawnPoints;
			bots.Add (b);
			/*
			int spawnPoint = Random.Range (0, spawnPoints.Length);
			GameObject b = Instantiate (bot, spawnPoints[spawnPoint].position, spawnPoints[spawnPoint].rotation) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
			b.GetComponent<BotMovement> ().waypoints = spawnPoints;
			bots.Add (b);
			*/
		}
	}

	private Vector3 GenerateSpawnPoint(){
		float rX = Random.Range (-(X/2) + 1, X/2 - 1);
		float y = Random.Range (0, 15);
		float zOffset = spawnPoints[0].position.z;

		Vector3 scale = new Vector3 (0.3f, y + 1, 0.3f);

		Vector3 position = new Vector3 (rX, (spawnPoints[0].position.y + y/2) - 0.5f, zOffset);

		spawnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		spawnObject.transform.position = position;
		spawnObject.transform.localScale = scale;
		spawnObject.AddComponent<BoxCollider> ();
		//spawnObject.name = "Cube: " + iteration;

		y += spawnPoints[0].position.y + 0.75f;
		return new Vector3(rX, y, zOffset);
	}

	public float GetFitness(){
		float fitness = 0f;
		fitness += BotVitals.MAX_HITPOINTS * BotsKilled;
		foreach (GameObject b in bots) {
			fitness += BotVitals.MAX_HITPOINTS - b.GetComponent<BotVitals>().hitPoints;
		}
		return fitness;
	}

	public void KillBot(GameObject b){
		bots.Remove (b);
		BotsKilled++;
	}

	public void OnDestroy(){
		Destroy (spawnObject);
		foreach (GameObject b in bots) {
			Destroy (b);
		}

		CancelInvoke ();
	}
}