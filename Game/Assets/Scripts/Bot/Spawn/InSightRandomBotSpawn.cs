using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InSightRandomBotSpawn : MonoBehaviour, IBotSpawn
{
    public Transform[] SpawnPoints { get; set; }
    public float X { get; set; }
    public float Z { get; set; }

    private GameObject BotPrefab;
    public IList<GameObject> Bots;
    private float SpawnTime;
    private static int BotsToSpawn = 1;
    private int BotsKilled = 0;
    public GameObject Player { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    public void StartSpawning()
    {
        Bots = new List<GameObject>();
        BotPrefab = Resources.Load("BotPrefab") as GameObject;
        FirstSpawn();
        //SpawnTime = 3f;
        //InvokeRepeating ("Spawn", 0, SpawnTime);
    }

    private void FirstSpawn()
    {

        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane+5)); //will get the middle of the screen

        Vector3 screenPosition = Player.GetComponentInChildren<Camera>().ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), Player.GetComponentInChildren<Camera>().farClipPlane / 2));
        if (screenPosition.x < -(X / 2)) screenPosition.x = - (X / 2 - 1);
        else if (screenPosition.x > X / 2 - 1) screenPosition.x = X / 2 - 1;
        if (screenPosition.y < 0) screenPosition.y = 0;
        else if (screenPosition.y > 15) screenPosition.y = 15;
        screenPosition.z = SpawnPoints[0].position.z;

        //Vector3 spawnPosition = GenerateSpawnPoint();
        //GameObject b = Instantiate(BotPrefab, spawnPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
        GameObject b = Instantiate(BotPrefab, screenPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
        b.GetComponent<BotVitals>().bs = this;
        Bots.Add(b);
    }

    void Spawn()
    {
        if (Bots.Count < BotsToSpawn)
        {
            Vector3 spawnPosition = GenerateSpawnPoint();
            GameObject b = Instantiate(BotPrefab, spawnPosition, new Quaternion(0, 180, 0, 0)) as GameObject;
            b.GetComponent<BotVitals>().bs = this;
            Bots.Add(b);
        }
    }

    private Vector3 GenerateSpawnPoint()
    {
        float rX = Random.Range(-(X / 2) + 1, X / 2 - 1);
        float y = Random.Range(0, 15);
        float zOffset = SpawnPoints[0].position.z;

        y += SpawnPoints[0].position.y + 0.75f;
        return new Vector3(rX, y, zOffset);
    }

    public float GetFitness()
    {
        float fitness = 0f;
        fitness += BotVitals.MAX_HITPOINTS * BotsKilled;
        foreach (GameObject b in Bots)
        {
            fitness += BotVitals.MAX_HITPOINTS - b.GetComponent<BotVitals>().hitPoints;
        }
        /*if(fitness > 200)
        {
            Debug.Log("Fitness: " + fitness);
        }*/
        return fitness;
    }

    public void KillBot(GameObject b)
    {
        Bots.Remove(b);
        BotsKilled++;
        Spawn();
    }

    public void OnDestroy()
    {
        foreach (GameObject b in Bots)
        {
            Destroy(b);
        }

        //CancelInvoke();
    }
}