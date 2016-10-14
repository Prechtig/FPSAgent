using UnityEngine;
using System.Collections;

public class NEATArena : MonoBehaviour{
	private float y;
	private float x;
	private float z;
	private float WallHeight;
	private float WallThickness;

	public Transform[] PlayerSpawnPoints;
	public Transform[] BotSpawnPoints;

	//private RandomHorizontalPlayerSpawn PlayerSpawn;
	//private RandomHorizontalBotSpawn BotSpawn;
	private RandomPlayerSpawn PlayerSpawn;
	private RandomBotSpawn BotSpawn;

	private IList ArenaObjects;

	private static int yOffset;
	private static Object yOffsetLock = new Object();

	public GameObject WallPrefab;
	private float RunningFitness = 0f;
	private int RunningFitnessCount = 0;

	public void Init() {
		y = GetAndIncrementYOffset () * 100;
		x = 25;
		z = 25;
		WallHeight = 5;
		WallThickness = 0.1f;
		ArenaObjects = new ArrayList ();

		CreateArena (x, z, WallHeight, WallThickness);
		CreateLight ();
		SetSpawnPoints (x, z);

		SpawnPlayer ();
		SpawnEnemies ();
	}

	private static float GetAndIncrementYOffset(){
		lock(yOffsetLock){
			return yOffset++;
		}
	}

	public static void ResetYOffset(){
		yOffset = 0;
	}
	
	void FixedUpdate () {
		if (BotSpawn.Bots.Count > 0) {
			float angle = Mathf.PI;
			float k = 100f;
			float c = 2f;
			
			angle = Vector3.Angle (PlayerSpawn.Player.transform.forward, BotSpawn.Bots [0].transform.position - PlayerSpawn.Player.transform.position) * Mathf.Deg2Rad;
			RunningFitness += k / (1 + (angle * c));
			RunningFitnessCount++;
		}
	}

	private void CreateArena(float x, float z, float wallHeight, float wallThickness) {
		// Create floor
		CreateCube (x, 0.1f, z, 0, y, 0);

		// Create northern wall
		CreateCube (x, wallHeight, wallThickness, 0, wallHeight/2 + y, z/2);

		// Create eastern wall
		CreateCube (wallThickness, wallHeight, z, 0-(x/2), wallHeight/2 + y, 0);

		// Create western wall
		CreateCube (wallThickness, wallHeight, z, x/2, wallHeight/2 + y, 0);

		// Create southern wall
		CreateCube (x, wallHeight, wallThickness, 0, wallHeight/2 + y, 0-(z/2));
	}

	private void CreateCube(float sx, float sy, float sz, float px, float py, float pz) {
		Vector3 scale = new Vector3 (sx, sy, sz);
		Vector3 position = new Vector3 (px, py, pz);
		CreateCube (position, scale);
	}

	private void CreateCube(Vector3 position, Vector3 scale) {
//		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject cube = Instantiate(WallPrefab);
		cube.transform.position = position;
		cube.transform.localScale = scale;

		ArenaObjects.Add (cube);
	}

	private void CreateLight(){
		Vector3 position = new Vector3 (0, 15 + y, 0);
		GameObject obj = new GameObject ();
		obj.transform.position = position;
		Light l = obj.AddComponent<Light> ();
		l.type = LightType.Point;
		l.color = new Color (255/255.0f, 244/255.0f, 214/255.0f);
		l.shadows = LightShadows.None;
		l.range = 75;
		l.intensity = 2;
		obj.name = "Light";

		ArenaObjects.Add (obj);
	}

	private void SetSpawnPoints(float x, float z){
		PlayerSpawnPoints = new Transform[1];
		GameObject tempSpawnObject = (new GameObject("SpawnObject"));
		Transform a = tempSpawnObject.transform;
		a.Translate(new Vector3(0,1 + y,0));
		PlayerSpawnPoints[0] = a;

		BotSpawnPoints = new Transform[2];
		tempSpawnObject = (new GameObject("SpawnObject"));
		BotSpawnPoints[0] = tempSpawnObject.transform;
		tempSpawnObject = (new GameObject("SpawnObject"));
		BotSpawnPoints[1] = tempSpawnObject.transform;


		Vector3 vBot0 = new Vector3 (x / 2 - 2, 1 + y, z / 2 - 2);
		BotSpawnPoints [0].Translate (vBot0);
		Vector3 vBot1 = new Vector3 (-(x / 2) + 2, 1 + y, z / 2 - 2);
		BotSpawnPoints [1].Translate (vBot1);

		BotSpawnPoints [0].LookAt (vBot1);
		BotSpawnPoints [1].LookAt (vBot0);
	}

	public GameObject GetPlayer(){
		return PlayerSpawn.Player;
	}

	public void SpawnPlayer(){
		PlayerSpawn = gameObject.AddComponent<RandomPlayerSpawn>();
		PlayerSpawn.X = x;
		PlayerSpawn.Z = z;
		PlayerSpawn.SpawnPoints = PlayerSpawnPoints;
		PlayerSpawn.SpawnPlayer ();
	}

	public void SpawnEnemies(){
		BotSpawn = gameObject.AddComponent<RandomBotSpawn> ();
		BotSpawn.X = x;
		BotSpawn.Z = z;
		BotSpawn.SpawnPoints = BotSpawnPoints;
		BotSpawn.StartSpawning ();
	}

	public float GetFitness(){
		float fitness = 0f;

		//New fitness function, does not work with the HorizontalBotSpawn
		/*
		float angle = Mathf.PI;
		float k = 15f;
		float c = 2f;
		try{
			angle = Vector3.Angle (PlayerSpawn.Player.transform.forward, BotSpawn.Bots[0].transform.position - PlayerSpawn.Player.transform.position) * Mathf.Deg2Rad;
		} catch (System.Exception e){
			Debug.Log (e);
		}
		fitness = k / (1 + (angle * c));
		*/

		fitness += RunningFitness / RunningFitnessCount;
		return fitness + BotSpawn.GetFitness ();
	}

	public void OnDestroy(){
		if (ArenaObjects != null) {
			foreach (GameObject o in ArenaObjects) {
				if (o != null) {
					Destroy (o.gameObject);
				}
			}
		}

		foreach (Transform t in PlayerSpawnPoints) {
			if (t != null) {
				Destroy (t.gameObject);
			}
		}
		foreach (Transform t in BotSpawnPoints) {
			if (t != null) {
				Destroy (t.gameObject);
			}
		}
		Destroy (BotSpawn);
		Destroy (PlayerSpawn);
	}
}