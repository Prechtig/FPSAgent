using UnityEngine;
using System.Collections;

public class NEATArena : MonoBehaviour {
	private float y;
	public float x;
	public float z;
	public float wallHeight;
	public float wallThickness;

	public Transform playerSpawnPoint;
	public Transform[] botSpawnPoints;

	private PlayerSpawn ps;
	private BotSpawn bs;

	private IList arenaObjects;

	private static int yOffset;
	private static Object yOffsetLock = new Object();

	public GameObject wallPrefab;

	public void Init() {
		y = GetAndIncrementYOffset () * 100;
		x = 25;
		z = 25;
		wallHeight = 5;
		wallThickness = 0.1f;
		arenaObjects = new ArrayList ();

		CreateArena (x, z, wallHeight, wallThickness);
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

	// Update is called once per frame
	void Update () {
		
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
		GameObject cube = Instantiate(wallPrefab);
		cube.transform.position = position;
		cube.transform.localScale = scale;
		cube.AddComponent<MeshCollider> ();

		arenaObjects.Add (cube);
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

		arenaObjects.Add (obj);

		/*
		Vector3[] rotations = new Vector3[4];
		rotations[0] = new Vector3 (45.0f, 0, 0);
		rotations[1] = new Vector3 (135.0f, 0, 0);
		rotations[2] = new Vector3 (45.0f, 90.0f, 0);
		rotations[3] = new Vector3 (45.0f, 270.0f, 0);

		for (int i = 0; i < 4; i++) {
			GameObject obj = new GameObject ();
			obj.transform.Translate (lightVector);
			obj.transform.eulerAngles = rotations [i];
			Light l = obj.AddComponent<Light> ();
			l.type = LightType.Directional;
			l.color = new Color (255/255.0f, 244/255.0f, 214/255.0f);
			l.shadows = LightShadows.Soft;
			obj.name = "Light";
			//obj.hideFlags = HideFlags.HideInHierarchy;

			arenaObjects.Add (obj);
		}
		*/
	}

	private void SetSpawnPoints(float x, float z){
		GameObject tempSpawnObject = (new GameObject("SpawnObject"));
		Transform a = tempSpawnObject.transform;
		a.Translate(new Vector3(0,1 + y,0));
		playerSpawnPoint = a;

		botSpawnPoints = new Transform[2];
		tempSpawnObject = (new GameObject("SpawnObject"));
		botSpawnPoints[0] = tempSpawnObject.transform;
		tempSpawnObject = (new GameObject("SpawnObject"));
		botSpawnPoints[1] = tempSpawnObject.transform;


		Vector3 vBot0 = new Vector3 (x / 2 - 2, 1 + y, z / 2 - 2);
		botSpawnPoints [0].Translate (vBot0);
		Vector3 vBot1 = new Vector3 (-(x / 2) + 2, 1 + y, z / 2 - 2);
		botSpawnPoints [1].Translate (vBot1);

		botSpawnPoints [0].LookAt (vBot1);
		botSpawnPoints [1].LookAt (vBot0);
	}

	public GameObject GetPlayer(){
		return ps.player;
	}

	public void SpawnPlayer(){
		ps = gameObject.AddComponent<PlayerSpawn>();
		ps.spawnPoints = playerSpawnPoint;
		ps.SpawnPlayer ();
	}

	public void SpawnEnemies(){
		bs = gameObject.AddComponent<BotSpawn>();
		bs.spawnPoints = botSpawnPoints;
		bs.StartSpawning ();
	}

	public float GetFitness(){
		return bs.GetFitness ();
	}

	public void OnDestroy(){
		if (arenaObjects != null) {
			foreach (GameObject o in arenaObjects) {
				if (o != null) {
					Destroy (o.gameObject);
				}
			}
		}

		if (playerSpawnPoint != null) {
			Destroy (playerSpawnPoint.gameObject);
		}

		foreach (Transform t in botSpawnPoints) {
			if (t != null) {
				Destroy (t.gameObject);
			}
		}

		Destroy (ps);
		Destroy (bs);
	}
}