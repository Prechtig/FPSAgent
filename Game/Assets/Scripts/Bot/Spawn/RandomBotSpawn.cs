using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomBotSpawn : MonoBehaviour, IBotSpawn
{
	public Transform[] SpawnPoints{ get; set;}
	public float X{ get; set;}
	public float Z{ get; set;}

	private GameObject BotPrefab;
	public IList<GameObject> Bots;
	private float SpawnTime;
	private static int BotsToSpawn = 1;
	private int BotsKilled = 0;

	//private GameObject spawnObject;

	/*public BotSpawn (Transform[] spawnPoints){
		this.spawnPoints = spawnPoints;
	}*/

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
		Bots = new List<GameObject>();
		BotPrefab = Resources.Load ("BotPrefab") as GameObject;
		SpawnTime = 3f;
		//bot.GetComponent<BotMovement> ().waypoints = spawnPoints;
		//bm.waypoints = spawnPoints;
		//player = (GameObject)Instantiate(a, spawnPoints.position, spawnPoints.rotation);		SpawnPlayer ();
		InvokeRepeating ("Spawn", 0, SpawnTime);
	}

	void Spawn ()
	{
		if(Bots.Count < BotsToSpawn){
			Vector3 spawnPosition = GenerateSpawnPoint ();
			GameObject b = Instantiate (BotPrefab, spawnPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
			//b.GetComponent<BotMovement> ().waypoints = spawnPoints;
			Bots.Add (b);
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
		float zOffset = SpawnPoints[0].position.z;

		/*
		Vector3 scale = new Vector3 (0.3f, y + 1, 0.3f);

		Vector3 position = new Vector3 (rX, (spawnPoints[0].position.y + y/2) - 0.5f, zOffset);


		spawnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		spawnObject.transform.position = position;
		spawnObject.transform.localScale = scale;
		spawnObject.AddComponent<BoxCollider> ();
		*/
		y += SpawnPoints[0].position.y + 0.75f;
		return new Vector3(rX, y, zOffset);
	}

	public float GetFitness(){
		float fitness = 0f;
		fitness += BotVitals.MAX_HITPOINTS * BotsKilled;
		foreach (GameObject b in Bots) {
			fitness += BotVitals.MAX_HITPOINTS - b.GetComponent<BotVitals>().hitPoints;
		}
		return fitness;
	}

	public void KillBot(GameObject b){
		Bots.Remove (b);
		BotsKilled++;
	}

	public void OnDestroy(){
		//Destroy (spawnObject);
		foreach (GameObject b in Bots) {
			Destroy (b);
		}

		CancelInvoke ();
	}
}