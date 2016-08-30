using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
	public GameObject player;
	public Transform[] spawnPoints;

	// Use this for initialization
	void Start ()
	{
		SpawnBot ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SpawnBot()
	{
		int random = Random.Range(0, spawnPoints.Length);
		Instantiate (player, spawnPoints [random].position, spawnPoints [random].rotation);
	}
}