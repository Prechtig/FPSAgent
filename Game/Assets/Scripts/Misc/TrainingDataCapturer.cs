using UnityEngine;
using System.Collections;
using System;

public class TrainingDataCapturer : MonoBehaviour
{
	public int saveInterval;
	private int frameCounter = 0;

	public int botsToSave;

	public string cameraTag;
	private Camera playerCam;

	private GroundTruth groundTruthScript;


	// Use this for initialization
	void Start ()
	{
		groundTruthScript = GetComponent<GroundTruth> ();
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
				Debug.Log ("Width " + trainingData.GetScreenshot ().GetWidth ());
			}
		}
	}

	public TrainingData CaptureTrainingData() {
		float[] groundTruths = groundTruthScript.CalculateGroundTruths (playerCam, botsToSave);
		Screenshot screenshot = ScreenSnapper.SnapScreenshot (playerCam);
		return new TrainingData (groundTruths, screenshot);
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
