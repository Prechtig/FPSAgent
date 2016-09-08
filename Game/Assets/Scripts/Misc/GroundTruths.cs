using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GroundTruths : MonoBehaviour
{
	public float[] CalculateGroundTruths (Camera playerCam, int botsToSave)
	{
		IEnumerable<GameObject> closestBots = FindClosestBots (playerCam, botsToSave);

		float[] inputs = new float[4 * botsToSave];

		int counter = 0;
		foreach (GameObject bot in closestBots) {
			bool isWithinSight = GameObjectHelper.IsObjectWithinSight (playerCam, bot);
			inputs[counter++] = !isWithinSight ? 0 : GameObjectHelper.HorizontalAngleTo (playerCam.transform, bot.transform);
			inputs[counter++] = !isWithinSight ? 0 : GameObjectHelper.VerticalAngleTo (playerCam.transform, bot.transform);
			inputs[counter++] = !isWithinSight ? 0 : GameObjectHelper.DistanceTo (playerCam.transform, bot.transform);
			inputs[counter++] = !isWithinSight ? 0 : 1;
		}
		return inputs;
	}

	private IEnumerable<GameObject> FindClosestBots (Camera playerCam, int amountOfBotsToFind) {
		GameObject[] allBots = GameObject.FindGameObjectsWithTag ("Bot");
		return allBots.OrderBy (bot => GameObjectHelper.AngleTo (playerCam.transform, bot.transform))
					.Take (amountOfBotsToFind);

	}

//	private bool IsBotWithinSight (GameObject bot) {
//		// The bottom-left of the camera is (0,0); the top-right is (1,1). The z position is in world units from the camera.
//		var viewPos = playerCam.WorldToViewportPoint (bot.transform.position);
//
//		return 	0 <= viewPos.x && viewPos.x <= 1 &&
//				0 <= viewPos.y && viewPos.y <= 1 &&
//				0 <= viewPos.z;
//	}
//
//	private float AngleTo (GameObject bot) {
//		return Vector3.Angle (playerCam.transform.forward, DirectionTo (bot));
//	}
//
//	private float VerticalAngleTo (GameObject bot) {
//		float botY = DirectionTo (bot).normalized.y;
//		float playerY = playerCam.transform.forward.normalized.y;
//
//		return RadiansToDegree(Math.Asin (botY) - Math.Asin (playerY));
//	}
//
//	private float HorizontalAngleTo (GameObject bot) {
//		float angle = Vector3.Angle (playerCam.transform.forward, DirectionTo (bot));
//
//		switch (AngleDir (playerCam.transform.forward, DirectionTo (bot), Vector3.up)) {
//		case Direction.Left:
//			return -angle;
//		case Direction.Right:
//			return angle;
//		case Direction.Forward:
//			return 0;
//		}
//		throw new InvalidOperationException ();
//	}
//
//	private float DistanceTo(GameObject bot) {
//		return Vector3.Distance (playerCam.transform.position, bot.transform.position);
//	}
//
//	private Vector3 DirectionTo(GameObject bot) {
//		// Gets a vector that points from the player's position to the bot's.
//		return bot.transform.position - playerCam.transform.position;
//	}
//
//	private Direction AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
//	{
//		Vector3 perp = Vector3.Cross(fwd, targetDir);
//		float dir = Vector3.Dot(perp, up);
//
//		if (dir > 0.0f) {
//			return Direction.Right;
//		} else if (dir < 0.0f) {
//			return Direction.Left;
//		} else {
//			return Direction.Forward;
//		}
//	}
//
//	private float RadiansToDegree(double radians) {
//		return (float) (radians * 180 / Math.PI);
//	}
}


