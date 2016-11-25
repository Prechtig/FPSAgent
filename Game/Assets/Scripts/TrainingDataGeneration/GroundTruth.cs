using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.TrainingDataGeneration;

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

	public static double[] CalculateGroundTruthsScaled (Camera playerCam, int botsToSave) {
		IEnumerable<GameObject> closestBots = FindClosestBots (playerCam, botsToSave);
		double[] inputs = new double[4 * botsToSave];

		int counter = 0;
		foreach (GameObject bot in closestBots) {
			bool isWithinSight = GameObjectHelper.IsObjectWithinSight (playerCam, bot);
			inputs[counter++] = !isWithinSight ? 0 : ScaleAngle(GameObjectHelper.HorizontalAngleTo (playerCam.transform, bot.transform));
			inputs[counter++] = !isWithinSight ? 0 : ScaleAngle(GameObjectHelper.VerticalAngleTo (playerCam.transform, bot.transform));
			inputs[counter++] = !isWithinSight ? 0 : ScaleDistance(GameObjectHelper.DistanceTo (playerCam.transform, bot.transform));
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

	public static double[] CalculateFeatures(Camera playerCam, GameObject bot) {
		//GameObject bot = FindClosestBots (playerCam, 1).First();
		bool withinSight = GameObjectHelper.IsObjectWithinSight (playerCam, bot);
		double horizontalAngle = GameObjectHelper.HorizontalAngleTo (playerCam.transform, bot.transform);
		double verticalAngle = GameObjectHelper.VerticalAngleTo (playerCam.transform, bot.transform);
		return CalculateFeatures (horizontalAngle, verticalAngle);
	}

	public static double[] CalculateFeatures(double horizontalAngle, double verticalAngle) {
		int numberOfPartitions = VisualPartitionClassifier.GetInstance ().GetNumberOfPartitions ();

		double[] result = new double[numberOfPartitions + 1];
		// Default to last index
		int index = numberOfPartitions;
        PartitionId partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle);
        if(partitionId != null)
        {
            index = VisualPartitionClassifier.GetInstance().CalculateFeatureIndexFromPartitionId(partitionId);
        }
        result[index] = 1d;
        return result;
	}
}
