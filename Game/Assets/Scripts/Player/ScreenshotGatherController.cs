using UnityEngine;
using System.Collections;

public class ScreenshotGatherController : MonoBehaviour {

	public const double CHANGE_DIRECTION_CHANCE = 0.01;
	public static System.Random rng = new System.Random();
	public static float modifier = 1;
	public static float increment = 0.01f;
	public static float counter = 0f;


	// Use this for initialization
	void Start () {
		transform.Rotate (new Vector3(-18f, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
		if (rng.NextDouble () < CHANGE_DIRECTION_CHANCE) {
			modifier = modifier * (-1);
		}
		transform.Rotate (0, modifier * 100f * Time.deltaTime, 0, Space.World);

		counter = counter + increment;
		transform.Rotate (new Vector3(Mathf.Sin(counter)/6, 0, 0));
	}
}
