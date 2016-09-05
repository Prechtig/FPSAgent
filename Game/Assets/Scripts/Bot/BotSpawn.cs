using UnityEngine;
using System.Collections;

public class BotSpawn : MonoBehaviour
{
	public GameObject bot;
	public Transform[] spawnPoints;
	public float spawnTime = 3f;
	public int botsToSpawn;

	// Use this for initialization
	void Start ()
	{
		InvokeRepeating ("Spawn", 0, spawnTime);
	}


	void Spawn ()
	{
		if(BotVitals.botsAlive < botsToSpawn){
			int spawnPoint = Random.Range (0, spawnPoints.Length);
			Instantiate (bot, spawnPoints[spawnPoint].position, spawnPoints[spawnPoint].rotation);
		}
	}
}

