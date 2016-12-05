using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.TrainingDataGeneration;

public class GroundTruth : MonoBehaviour
{
	private static readonly double MAX_DISTANCE = 50;

	private static int _fov = -1;
	private static int Fov{
		get{
			if (_fov == -1) {
				_fov = int.Parse( PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("topology.fov"));
			}
			return _fov;
		}
	}

	public static double[] CalculateGroundTruths (Camera playerCam, int botsToSave) {
		IEnumerable<GameObject> closestBots = FindClosestBots (playerCam, botsToSave);
        double[] inputs = new double[4 * botsToSave];

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
		return angle / (Fov / 2);
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
