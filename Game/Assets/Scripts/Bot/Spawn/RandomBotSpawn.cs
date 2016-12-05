using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomBotSpawn : MonoBehaviour, IBotSpawn
{
	public Transform[] SpawnPoints{ get; set;}
	public float X{ get; set;}
	public float Z{ get; set;}
    public GameObject Player { get; set; }

    private GameObject BotPrefab;
	public IList<GameObject> Bots;
	//private float SpawnTime;
    private static int BotsToSpawn;
	private int BotsKilled = 0;

	// Use this for initialization
	void Start ()
	{
        
    }

	public void StartSpawning(){
        BotsToSpawn = int.Parse(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.bots"));
        Bots = new List<GameObject>();
		BotPrefab = Resources.Load ("BotPrefab") as GameObject;
        for (int i = 0; i < BotsToSpawn; i++)
        {
            Spawn();
        }

		//SpawnTime = 3f;
		//InvokeRepeating ("Spawn", 0, SpawnTime);
	}

	void Spawn ()
	{
		if(Bots.Count < BotsToSpawn){
			Vector3 spawnPosition = GenerateSpawnPoint ();
			GameObject b = Instantiate (BotPrefab, spawnPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
			b.GetComponent<BotVitals> ().bs = this;
            if (bool.Parse(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.enableMovement"))) {
                BotMovement bm = b.AddComponent<BotMovement>();
                bm.X = X;
                bm.zOffset = SpawnPoints[0].position.z;
                bm.yOffset = SpawnPoints[0].position.y;
            }
			Bots.Add (b);
		}
	}

	public Vector3 GenerateSpawnPoint(){
		float rX = Random.Range (-(X/2) + 1, X/2 - 1);
		float y = Random.Range (0, 15);
		float zOffset = SpawnPoints[0].position.z;

		y += SpawnPoints[0].position.y + 0.75f;
		return new Vector3(rX, y, zOffset);
	}

	public float GetFitness(){
		float fitness = 0f;
		fitness += BotVitals.MAX_HITPOINTS * BotsKilled;
        if (BotsToSpawn > 1)
        {
            fitness += 25 * BotsKilled;
        }
		foreach (GameObject b in Bots) {
			fitness += BotVitals.MAX_HITPOINTS - b.GetComponent<BotVitals>().hitPoints;
		}
		return fitness;
	}

	public void KillBot(GameObject b){
		Bots.Remove (b);
		BotsKilled++;
		Spawn ();
	}

	public void OnDestroy(){
		foreach (GameObject b in Bots) {
			Destroy (b);
		}

		//CancelInvoke ();
	}
}