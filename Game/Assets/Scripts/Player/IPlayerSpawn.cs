using UnityEngine;
using System.Collections;

public interface IPlayerSpawn
{
	GameObject player{ get; set; }
	Transform[] spawnPoints{ get; set; }
	float X{ get; set; }
	float Z{ get; set; }

	void SpawnPlayer();

	// Use this for initialization
	void Start ();
	
	// Update is called once per frame
	void Update ();
}

