using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
	public GameObject player;
	public Transform spawnPoints;

	public void OnEnable(){
		//SpawnPlayer ();
	}

	// Use this for initialization
	/*
	void Start ()
	{
		SpawnPlayer ();
	}
	*/

	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SpawnPlayer()
	{
		/*
		int random = Random.Range(0, spawnPoints.Length);
		Instantiate (player, spawnPoints [random].position, spawnPoints [random].rotation);
		*/
		//Object[] obs = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		Object a = Resources.Load ("Player");
		player = (GameObject)Instantiate(a, spawnPoints.position, spawnPoints.rotation);
		//Instantiate (GameObject., spawnPoints.position, spawnPoints.rotation);
	}

	/*
	public void OnDestroy(){
		Destroy (spawnPoints);
	}
	*/
}