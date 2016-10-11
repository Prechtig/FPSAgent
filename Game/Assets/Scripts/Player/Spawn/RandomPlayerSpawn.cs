using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSpawn : MonoBehaviour, IPlayerSpawn
{
	public GameObject Player{ get; set;}
	public Transform[] SpawnPoints{ get; set;}
	public float X{ get; set;}
	public float Z{ get; set;}

	//private GameObject spawnObject;

	private static Text _shots;

	//Canvas components
	private static Canvas _hud;
	private static Canvas HUD{
		get {
			if (_hud == null) {
				_hud = GameObject.FindWithTag("PlayerHUD").GetComponent<Canvas>() as Canvas;
			}
			return _hud;
		}
	}

	private static Text _shotsLeftText;
	private static Text ShotsLeftText{
		get {
			if (_shotsLeftText == null) {
				_shotsLeftText = GameObject.FindWithTag("UIText").GetComponent<Text>() as Text;
			}
			return _shotsLeftText;
		}
	}

	public void SpawnPlayer()
	{
		/*
		int random = Random.Range(0, spawnPoints.Length);
		Instantiate (player, spawnPoints [random].position, spawnPoints [random].rotation);
		*/
		//Object[] obs = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		Object a = Resources.Load ("Player");

		Vector3 spawnPosition = GenerateSpawnPoint ();
		Player = (GameObject)Instantiate(a, spawnPosition, SpawnPoints[0].rotation);
		//HUD.worldCamera = player.GetComponentInChildren<Camera> ();
		Player.GetComponentInChildren<NEATWeapon> ().ShotsLeftText = ShotsLeftText;
		//Instantiate (GameObject., spawnPoints.position, spawnPoints.rotation);
	}

	private Vector3 GenerateSpawnPoint(){
		float rX = Random.Range (-(X/2) + 1, X/2 - 1);
		float y = Random.Range (1, 15);
		float zOffset = -4;

		/*
		Vector3 scale = new Vector3 (0.3f, y + 1, 0.3f);
		
		Vector3 position = new Vector3 (rX, (SpawnPoints[0].position.y + y/2) - 0.5f, zOffset);
		
		spawnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		spawnObject.transform.position = position;
		spawnObject.transform.localScale = scale;
		spawnObject.AddComponent<BoxCollider> ();
		*/

		y += SpawnPoints[0].position.y - 0.5f;
		return new Vector3(rX, y, zOffset);
	}


	public void OnDestroy(){
		//Destroy (spawnObject);
	}
}