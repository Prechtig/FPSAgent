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

	private GroundTruths groundTruthsScript;


	// Use this for initialization
	void Start ()
	{
		groundTruthsScript = GetComponent<GroundTruths> ();
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
		float[] groundTruths = groundTruthsScript.CalculateGroundTruths (playerCam, botsToSave);
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
