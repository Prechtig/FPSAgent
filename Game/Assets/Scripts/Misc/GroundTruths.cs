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

	public GameObject player;
	public Camera playerCam;


	private int frameCounter = 0;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		frameCounter = ++frameCounter % saveInterval;

		if (frameCounter == 0) {
			float[] botInfo = GetBotInfo ();

			string fileName = GenerateFileName ();

			System.IO.File.WriteAllLines (fileName + ".dat", botInfo.Select( f => f.ToString()).ToArray(), System.Text.Encoding.UTF8);
			Application.CaptureScreenshot (fileName + ".png");
		}
	}

	private float[] GetBotInfo ()
	{
		var closestBots = FindClosestBots (botsToSave);

		float[] inputs = new float[botsToSave * 4];

		int counter = 0;
		foreach (GameObject bot in closestBots) {
			inputs[counter++] = HorizontalAngleTo (bot);
			inputs[counter++] = VerticalAngleTo (bot);
			inputs[counter++] = DistanceTo (bot);
			inputs[counter++] = IsBotWithinSight (bot) ? 1 : 0;
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
		return Vector3.Angle (player.transform.rotation.eulerAngles, DirectionTo (bot));
	}

	private float VerticalAngleTo (GameObject bot) {
		return Vector3.Angle (player.transform.forward, Vector3.up);
	}

	private float HorizontalAngleTo (GameObject bot) {
		float playerY = player.transform.rotation.eulerAngles.y;
		float botY = DirectionTo (bot).y;
		return playerY - botY;
	}

	private float DistanceTo(GameObject bot) {
		return Vector3.Distance (player.transform.position, bot.transform.position);
	}

	private Vector3 DirectionTo(GameObject bot) {
		// Gets a vector that points from the player's position to the bot's.
		return bot.transform.position - player.transform.position;
	}

	private string GenerateFileName() {
		return "groundtruths/" + fileNamePrefix + "_" + DateTime.Now.ToString ("yyyy-MM-dd_hh-mm-ss-fff");
	}
}
