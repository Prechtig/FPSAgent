using UnityEngine;
using System.Collections;

public class TrainingData {

	private readonly float[] groundTruths;
	private readonly Screenshot screenshot;

	public TrainingData(float[] groundTruths, Screenshot screenshot) {
		this.groundTruths = groundTruths;
		this.screenshot = screenshot;
	}

	public float[] GetGroundTruths() {
		return groundTruths;
	}

	public Screenshot GetScreenshot() {
		return screenshot;
	}

}
