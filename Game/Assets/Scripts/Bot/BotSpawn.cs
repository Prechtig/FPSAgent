using UnityEngine;
using System.Collections;

public class BotSpawn : MonoBehaviour
{
	public GameObject bot;
	public Transform[] spawnPoints;

	// Use this for initialization
	void Start ()
	{
		SpawnBot (); 

	}

	public void SpawnBot()
	{
		int random = Random.Range(0, spawnPoints.Length);
		Instantiate (bot, spawnPoints [random].position, spawnPoints [random].rotation);
	}
}

