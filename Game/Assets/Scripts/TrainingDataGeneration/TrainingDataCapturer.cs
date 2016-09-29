using UnityEngine;
using System.Collections;
using System;
using Kajabity.Tools.Java;

public class TrainingDataCapturer : MonoBehaviour
{
	private static int saveInterval;
	private int frameCounter = 0;

	private static int bots;

	private string cameraTag = "MainCamera";
	private Camera playerCam;

	static TrainingDataCapturer() {
		JavaProperties projectProperties = PropertiesReader.GetPropertyFile (PropertyFile.Project);
		bots = int.Parse (projectProperties.GetProperty ("game.bots"));
		saveInterval = int.Parse(projectProperties.GetProperty("datageneration.screenshot.save.interval"));
	}

	// Use this for initialization
	void Start ()
	{
		DatabaseWriter.Initialize ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!IsCameraInitialized ()) {
			InitializeCamera ();
		} else {
			frameCounter = ++frameCounter % saveInterval;

			if (frameCounter == 0) {
				TrainingData trainingData = CaptureTrainingData ();
				DatabaseWriter.InsertTrainingData (trainingData);
			}
		}
	}

	public TrainingData CaptureTrainingData() {
		float[] groundTruths = GroundTruth.CalculateGroundTruths (playerCam, bots);
		Screenshot screenshot = ScreenSnapper.SnapScreenshot (playerCam);
		return new TrainingData (bots, groundTruths, screenshot);
	}

	private bool IsCameraInitialized() {
		return playerCam != null;
	}

	private void InitializeCamera() {
		GameObject cam = GameObject.FindGameObjectWithTag (cameraTag);
		if(cam != null) {
			playerCam = cam.GetComponent<Camera> ();
		}
	}
}
