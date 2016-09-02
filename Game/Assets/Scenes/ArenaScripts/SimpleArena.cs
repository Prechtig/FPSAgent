using UnityEngine;
using System.Collections;

public class SimpleArena : MonoBehaviour {

	private const float defaultWallThickness = 0.2f;
	public Transform playerSpawnPoint;
	public Transform[] botSpawnPoints;

	// Use this for initialization
	void Start () {
		float x = 25;
		float z = 25;
		float wallHeight = 5;
		float wallThickness = 0.1f;
		CreateArena (x, z, wallHeight, wallThickness);
		SetSpawnPoints (x, z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	private void CreateArena(float x, float z, float wallHeight, float wallThickness) {
		// Create floor
		CreateCube (x, 0.1f, z, 0, 0, 0);

		// Create northern wall
		CreateCube (x, wallHeight, wallThickness, 0, wallHeight/2, z/2);

		// Create eastern wall
		CreateCube (wallThickness, wallHeight, z, 0-(x/2), wallHeight/2, 0);

		// Create western wall
		CreateCube (wallThickness, wallHeight, z, x/2, wallHeight/2, 0);

		// Create southern wall
		CreateCube (x, wallHeight, wallThickness, 0, wallHeight/2, 0-(z/2));

		
	}

	private void CreateCube(float sx, float sy, float sz, float px, float py, float pz) {
		Vector3 scale = new Vector3 (sx, sy, sz);
		Vector3 position = new Vector3 (px, py, pz);
		CreateCube (position, scale);
	}

	private void CreateCube(Vector3 position, Vector3 scale) {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = position;
		cube.transform.localScale = scale;
		cube.AddComponent<MeshCollider> ();
	}

	private void SetSpawnPoints(float x, float z){
		Debug.Log (-(z / 2) + 1);
		Vector3 v = new Vector3 (0, 1, -(z / 2) + 1);
		playerSpawnPoint.Translate(v);

		Vector3 vBot0 = new Vector3 (x / 2 - 2, 1, z / 2 - 2);
		botSpawnPoints [0].Translate (vBot0);
		Vector3 vBot1 = new Vector3 (-(x / 2) + 2, 1, z / 2 - 2);
		botSpawnPoints [1].Translate (vBot1);

		botSpawnPoints [0].LookAt (vBot1);
		botSpawnPoints [1].LookAt (vBot0);
	}
}
