using UnityEngine;
using System.Collections;

public class TrainingData {

	private float[] groundTruths;
	private Screenshot screenshot;

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
