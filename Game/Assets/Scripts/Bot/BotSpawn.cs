using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotSpawn : MonoBehaviour
{
	public GameObject bot;
	private IList<GameObject> bots;
	public Transform[] spawnPoints;
	private float spawnTime;
	private static int botsToSpawn = 3;
	private int BotsKilled = 0;

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
		bot = Resources.Load ("BotPreFab") as GameObject;
		spawnTime = 3f;
		//bot.GetComponent<BotMovement> ().waypoints = spawnPoints;
		//bm.waypoints = spawnPoints;
		//player = (GameObject)Instantiate(a, spawnPoints.position, spawnPoints.rotation);
		InvokeRepeating ("Spawn", 0, spawnTime);
	}

	void Spawn ()
	{
		if(bots.Count < botsToSpawn){
			int spawnPoint = Random.Range (0, spawnPoints.Length);
			GameObject b = Instantiate (bot, spawnPoints[spawnPoint].position, spawnPoints[spawnPoint].rotation) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
			b.GetComponent<BotMovement> ().waypoints = spawnPoints;
			bots.Add (b);
		}
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
		CancelInvoke ();
		foreach (GameObject b in bots) {
			Destroy (b);
		}
		/*foreach (Transform t in spawnPoints) {
			Destroy (t);
		}*/
	}
}

