using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GroundTruth : MonoBehaviour
{
	public float[] CalculateGroundTruths (Camera playerCam, int botsToSave) {
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
}
