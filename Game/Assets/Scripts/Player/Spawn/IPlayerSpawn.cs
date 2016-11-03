using UnityEngine;
using System.Collections;

public interface IPlayerSpawn
{
	GameObject Player { get; set; }
	NEATArena Arena { get; set; }
	Transform[] SpawnPoints { get; set; }
	float X { get; set; }
	float Z { get; set; }

	void SpawnPlayer();
}