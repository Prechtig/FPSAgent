using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GroundTruths : MonoBehaviour
{

	public int saveInterval;
	public int botsToSave;

	public string botTag;
	public string fileNamePrefix;

	public Camera playerCam;

	private int frameCounter = 0;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerCam != null) {
			frameCounter = ++frameCounter % saveInterval;

			if (frameCounter == 0) {
				float[] botInfo = GetBotInfo ();

				string fileName = GenerateFileName ();

				System.IO.File.WriteAllLines (fileName + ".dat", botInfo.Select( f => f.ToString()).ToArray(), System.Text.Encoding.UTF8);
				Application.CaptureScreenshot (fileName + ".png");
			}
		} else {
			playerCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		}
	}

	private float[] GetBotInfo ()
	{
		var closestBots = FindClosestBots (botsToSave);

		float[] inputs = new float[botsToSave * 4];

		int counter = 0;
		foreach (GameObject bot in closestBots) {
			bool isWithinSight = IsBotWithinSight (bot);
			inputs[counter++] = !isWithinSight ? 0 : HorizontalAngleTo (bot);
			inputs[counter++] = !isWithinSight ? 0 : VerticalAngleTo (bot);
			inputs[counter++] = !isWithinSight ? 0 : DistanceTo (bot);
			inputs[counter++] = !isWithinSight ? 0 : 1;
		}
		return inputs;
	}

	private IEnumerable<GameObject> FindClosestBots (int amountOfBotsToFind) {
		GameObject[] allBots = GameObject.FindGameObjectsWithTag (botTag);
		return allBots.OrderBy (bot => AngleTo (bot))
					.Take (amountOfBotsToFind);

	}

	private bool IsBotWithinSight (GameObject bot) {
		// The bottom-left of the camera is (0,0); the top-right is (1,1). The z position is in world units from the camera.
		var viewPos = playerCam.WorldToViewportPoint (bot.transform.position);

		return 	0 <= viewPos.x && viewPos.x <= 1 &&
				0 <= viewPos.y && viewPos.y <= 1 &&
				0 <= viewPos.z;
	}

	private float AngleTo (GameObject bot) {
		return Vector3.Angle (playerCam.transform.forward, DirectionTo (bot));
	}

	private float VerticalAngleTo (GameObject bot) {
		float botY = DirectionTo (bot).normalized.y;
		float playerY = playerCam.transform.forward.normalized.y;

		return RadiansToDegree(Math.Asin (botY) - Math.Asin (playerY));
	}

	private float HorizontalAngleTo (GameObject bot) {
		float angle = Vector3.Angle (playerCam.transform.forward, DirectionTo (bot));

		switch (AngleDir (playerCam.transform.forward, DirectionTo (bot), Vector3.up)) {
		case Direction.Left:
			return -angle;
		case Direction.Right:
			return angle;
		case Direction.Forward:
			return 0;
		}
		throw new InvalidOperationException ();
	}

	private float DistanceTo(GameObject bot) {
		return Vector3.Distance (playerCam.transform.position, bot.transform.position);
	}

	private Vector3 DirectionTo(GameObject bot) {
		// Gets a vector that points from the player's position to the bot's.
		return bot.transform.position - playerCam.transform.position;
	}

	private string GenerateFileName() {
		return "groundtruths/" + fileNamePrefix + "_" + DateTime.Now.ToString ("yyyy-MM-dd_hh-mm-ss-fff");
	}

	private Direction AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);

		if (dir > 0.0f) {
			return Direction.Right;
		} else if (dir < 0.0f) {
			return Direction.Left;
		} else {
			return Direction.Forward;
		}
	}

	private float RadiansToDegree(double radians) {
		return (float) (radians * 180 / Math.PI);
	}
}

enum Direction { Left, Right, Forward };
