using UnityEngine;
using System.Collections;

public interface IPlayerSpawn
{
	GameObject Player{ get; set; }
	Transform[] SpawnPoints{ get; set; }
	float X{ get; set; }
	float Z{ get; set; }

	void SpawnPlayer();
}