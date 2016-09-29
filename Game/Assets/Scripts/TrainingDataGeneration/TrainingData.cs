using UnityEngine;
using System.Collections;

public class TrainingData {

	private readonly int numberOfBots;
	private readonly float[] groundTruths;
	private readonly Screenshot screenshot;

	public TrainingData(int numberOfBots, float[] groundTruths, Screenshot screenshot) {
		this.numberOfBots = numberOfBots;
		this.groundTruths = groundTruths;
		this.screenshot = screenshot;
	}

	public int GetNumberOfBots() {
		return numberOfBots;
	}

	public float[] GetGroundTruths() {
		return groundTruths;
	}

	public Screenshot GetScreenshot() {
		return screenshot;
	}

}
