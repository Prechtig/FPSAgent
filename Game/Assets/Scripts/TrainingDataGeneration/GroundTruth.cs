using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GroundTruth : MonoBehaviour
{
	private static readonly double ANGLE_OFFSET = 90;
	private static readonly double ANGLE_SCALE_FACTOR = 180;
	private static readonly double MAX_DISTANCE = 50;


	public static float[] CalculateGroundTruths (Camera playerCam, int botsToSave) {
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

	public static float[] CalculateGroundTruthsScaled (Camera playerCam, int botsToSave) {
		IEnumerable<GameObject> closestBots = FindClosestBots (playerCam, botsToSave);
		float[] inputs = new float[4 * botsToSave];

		int counter = 0;
		foreach (GameObject bot in closestBots) {
			bool isWithinSight = GameObjectHelper.IsObjectWithinSight (playerCam, bot);
			inputs[counter++] = !isWithinSight ? 0 : (float)ScaleAngle(GameObjectHelper.HorizontalAngleTo (playerCam.transform, bot.transform));
			inputs[counter++] = !isWithinSight ? 0 : (float)ScaleAngle(GameObjectHelper.VerticalAngleTo (playerCam.transform, bot.transform));
			inputs[counter++] = !isWithinSight ? 0 : (float)ScaleDistance(GameObjectHelper.DistanceTo (playerCam.transform, bot.transform));
			inputs[counter++] = !isWithinSight ? 0 : 1;
		}
		return inputs;
	}

	private static IEnumerable<GameObject> FindClosestBots (Camera playerCam, int amountOfBotsToFind) {
		GameObject[] allBots = GameObject.FindGameObjectsWithTag ("Bot");
		return allBots.OrderBy (bot => GameObjectHelper.AngleTo (playerCam.transform, bot.transform))
					.Take (amountOfBotsToFind);

	}

	private static double ScaleAngle(double angle) {
		return (angle + ANGLE_OFFSET) / ANGLE_SCALE_FACTOR;
	}

	private static double ScaleDistance(double distance) {
		if (distance > MAX_DISTANCE) distance = MAX_DISTANCE;
		return distance/MAX_DISTANCE;
	}
}
