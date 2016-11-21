using UnityEngine;
using System.Collections;

public interface IBotSpawn
{
	Transform[] SpawnPoints{ get; set; }
	float X{ get; set; }
	float Z{ get; set; }
    GameObject Player { get; set; }

	void StartSpawning();
	float GetFitness();
	void KillBot(GameObject b);
}

