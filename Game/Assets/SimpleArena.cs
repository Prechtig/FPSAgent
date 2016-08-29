using UnityEngine;
using System.Collections;

public class SimpleArena : MonoBehaviour {

	private const float defaultWallThickness = 0.2f;

	// Use this for initialization
	void Start () {
//		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//		cube.AddComponent<Rigidbody>();
//		cube.transform.position = new Vector3 (1, 1, 1);
//		cube.transform.localScale = new Vector3 (10, 3, defaultWallThickness);

		CreateArena (25, 25, 5, 0.1f);
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
//		cube.AddComponent<Rigidbody>();
		cube.transform.position = position;
		cube.transform.localScale = scale;
		cube.AddComponent<MeshCollider> ();
	}
}
