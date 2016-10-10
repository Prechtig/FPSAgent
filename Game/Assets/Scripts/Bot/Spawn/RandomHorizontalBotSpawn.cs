using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefaultHorizontalBotSpawn : MonoBehaviour, IBotSpawn
{
	public Transform[] SpawnPoints{ get; set;}
	public float X{ get; set;}
	public float Z{ get; set;}

	private GameObject Bot;
	private IList<GameObject> Bots;
	private float SpawnTime;
	private static int BotsToSpawn = 1;
	private int BotsKilled = 0;

	public void StartSpawning(){
		Bots = new List<GameObject>();
		Bot = Resources.Load ("BotPrefab") as GameObject;
		SpawnTime = 3f;
		InvokeRepeating ("Spawn", 0, SpawnTime);
	}

	void Spawn ()
	{
		if(Bots.Count < BotsToSpawn){
			int spawnPoint = Random.Range (0, SpawnPoints.Length);
			GameObject b = Instantiate (Bot, SpawnPoints[spawnPoint].position, SpawnPoints[spawnPoint].rotation) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
			b.GetComponent<BotMovement> ().waypoints = SpawnPoints;
			Bots.Add (b);
		}
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
		CancelInvoke ();
		foreach (GameObject b in Bots) {
			Destroy (b);
		}
	}
}